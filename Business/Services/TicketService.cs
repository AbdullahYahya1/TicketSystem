using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TicketSystem.Business.IServices;
using TicketSystem.DataAccess.Context;
using TicketSystem.DataAccess.DTOs;
using TicketSystem.DataAccess.IRepositories;
using TicketSystem.DataAccess.Models;
using TicketSystem.Business;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Hangfire.MemoryStorage.Dto;
using System.Net.Sockets;

namespace TicketSystem.Business.Services
{
    public class TicketService : Service<Ticket>, ITicketService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TicketService> _logger;
        private readonly IEmailSender _emailSender;

        public TicketService(TicketSystemDbContext context, IUnitOfWork unitOfWork, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, ILogger<TicketService> logger,
            IEmailSender emailSender) : base(context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _emailSender = emailSender;
        }

        public async Task<ResponseModel<bool>> UpdateTicketStatusAsync(int ticketId, TicketStatus status)
        {
            try
            {
                var ticket = await _unitOfWork.tickets.GetByIdAsync(ticketId);
                if (ticket == null)
                {
                    return new ResponseModel<bool>
                    {
                        IsSuccess = false,
                        Message = "TicketNotFound",
                        Result = false
                    };
                }

                if (ticket.Status == TicketStatus.Closed || ticket.Status == TicketStatus.Canceled)
                {
                    return new ResponseModel<bool>
                    {
                        IsSuccess = false,
                        Message = "CannotUpdateATicketThatIsAlreadyClosedOrCanceled",
                        Result = false
                    };
                }

                var currentUserId = _httpContextAccessor.HttpContext.User?.FindFirst("UserId")?.Value;
                var currentUserRole = _httpContextAccessor.HttpContext.User?.FindFirst("UserType")?.Value;
                UserType userType = (UserType)Enum.Parse(typeof(UserType), currentUserRole);

                if (ticket.CreatedById != currentUserId && ticket.AssignedToId != currentUserId)
                {
                    return new ResponseModel<bool>
                    {
                        IsSuccess = false,
                        Message = "Unauthorized",
                        Result = false
                    };
                }
                if (ticket.Status == TicketStatus.New || ticket.AssignedToId == null)
                {
                    return new ResponseModel<bool>
                    {
                        IsSuccess = false,
                        Message = "CannotUpdateANewOrUnassignedTicket",
                        Result = false
                    };
                }
                if (status == TicketStatus.Confirmed && userType != UserType.Client)
                {
                    return new ResponseModel<bool>
                    {
                        IsSuccess = false,
                        Message = "OnlyClientsCanConfirm",
                        Result = false
                    };
                }

                if (status == TicketStatus.Closed && ticket.Status != TicketStatus.Confirmed && userType == UserType.Support)
                {
                    return new ResponseModel<bool>
                    {
                        IsSuccess = false,
                        Message = "SupportCannotCloseUnconfirmedTicket",
                        Result = false
                    };
                }

                ticket.Status = status;
                ticket.UpdateDate = DateTime.UtcNow;
                ticket.UpdateById = currentUserId;
                await _unitOfWork.tickets.UpdateAsync(ticket);

                var ticketDetail = new TicketDetail
                {
                    TicketId = ticketId,
                    CreateById = currentUserId,
                    Status = status.ToString(),
                    CreatedDate = DateTime.UtcNow
                };

                await _unitOfWork.ticketDetails.AddAsync(ticketDetail);
                await _unitOfWork.SaveChangesAsync();

                if (userType == UserType.Client)
                {
                    var supportUser = await _unitOfWork.users.GetUserById(ticket.AssignedToId);
                    await _emailSender.SendEmailAsync(
                        supportUser.Email,
                        "Ticket Status Updated",
                        $"The status of ticket #{ticketId} has been updated to {status} by the client."
                    );
                }
                else if (userType == UserType.Support)
                {
                    var clientUser = await _unitOfWork.users.GetUserById(ticket.CreatedById);
                    await _emailSender.SendEmailAsync(
                        clientUser.Email,
                        "Ticket Status Updated",
                        $"The status of your ticket #{ticketId} has been updated to {status} by support."
                    );
                }

                return new ResponseModel<bool>
                {
                    IsSuccess = true,
                    Message = string.Empty,
                    Result = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the ticket status.");
                return new ResponseModel<bool>
                {
                    IsSuccess = false,
                    Message = "ErrorFound",
                    Result = false
                };
            }
        }

        public async Task<ResponseModel<GetOneTicketDto>> GetTicketByIdAsync(int ticketId)
        {
            try
            {
                var ticket = await _unitOfWork.tickets.GetTicketWithDetailsAsync(ticketId);

                if (ticket == null)
                {
                    return new ResponseModel<GetOneTicketDto>
                    {
                        IsSuccess = false,
                        Message = "TicketNotFound"
                    };
                }

                var currentUserId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
                var currentUserRole = _httpContextAccessor.HttpContext.User?.FindFirst("UserType")?.Value;
                UserType userType = (UserType)Enum.Parse(typeof(UserType), currentUserRole);


                if (!(ticket.CreatedById == currentUserId || ticket.AssignedToId == currentUserId || userType == UserType.Manager))
                {
                    return new ResponseModel<GetOneTicketDto>
                    {
                        IsSuccess = false,
                        Message = "AccessDenied"
                    };
                }

                var ticketDto = _mapper.Map<GetOneTicketDto>(ticket);
                ticketDto.Attachments = [];
                foreach (var attachment in ticket.Attachments)
                {
                    if (File.Exists(attachment.FilePath))
                    {
                        byte[] imageBytes = await File.ReadAllBytesAsync(attachment.FilePath);
                        var base64 = Convert.ToBase64String(imageBytes);
                        ticketDto.Attachments.Add(base64);
                    }
                }
                return new ResponseModel<GetOneTicketDto>
                {
                    Result = ticketDto,
                    IsSuccess = true,
                    Message = string.Empty
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the ticket.");
                return new ResponseModel<GetOneTicketDto>
                {
                    IsSuccess = false,
                    Message = "ErrorFound"
                };
            }
        }

        public async Task<ResponseModel<bool>> AssignTicketToEmployeeAsync(int ticketId, string employeeId)
        {
            try
            {
                var employee = await _unitOfWork.users.GetUserById(employeeId);
                if (employee == null)
                {
                    _logger.LogWarning($"AssignTicketToEmployeeAsync failed: Employee with id {employeeId} not found.");
                    return new ResponseModel<bool>
                    {
                        IsSuccess = false,
                        Message = "EmployeeNotFound",
                        Result = false
                    };
                }
                var currentUserId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
                var ticket = await _unitOfWork.tickets.GetByIdAsync(ticketId);
                if (ticket != null)
                {
                    if (!string.IsNullOrEmpty(ticket.AssignedToId))
                    {
                        _logger.LogWarning($"AssignTicketToEmployeeAsync failed: Ticket with ID {ticketId} is already assigned to another employee.");
                        return new ResponseModel<bool>
                        {
                            IsSuccess = false,
                            Message = "TicketAlreadyAssigned",
                            Result = false
                        };
                    }
                    ticket.AssignedToId = employee.UserId.ToString();
                    ticket.Status = TicketStatus.Assigned;
                    ticket.UpdateDate = DateTime.UtcNow;
                    ticket.UpdateById = currentUserId;
                    await _unitOfWork.tickets.UpdateAsync(ticket);

                    var ticketDetail = new TicketDetail
                    {
                        TicketId = ticketId,
                        CreateById = currentUserId,
                        Status = TicketStatus.Assigned.ToString(),
                        CreatedDate = DateTime.UtcNow
                    };

                    await _unitOfWork.ticketDetails.AddAsync(ticketDetail);
                    await _unitOfWork.SaveChangesAsync();

                    await _emailSender.SendEmailAsync(
                    employee.Email,
                    "Ticket Assigned To You",
                    $"You've been assigned a new ticket with ticketId :  #{ticketId}. Please check it.");
                }
                else
                {
                    _logger.LogWarning($"AssignTicketToEmployeeAsync failed: Ticket with ID {ticketId} not found.");
                    return new ResponseModel<bool>
                    {
                        IsSuccess = false,
                        Message = "TicketNotFound",
                        Result = false
                    };
                }

                return new ResponseModel<bool>
                {
                    IsSuccess = true,
                    Message = string.Empty,
                    Result = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while assigning the ticket to an employee.");
                return new ResponseModel<bool>
                {
                    IsSuccess = false,
                    Message = "ErrorFound",
                    Result = false
                };
            }
        }


        public async Task<ResponseModel<GetCommentDto>> AddCommentToTicketAsync(int ticketId, PostCommentDto commentDto)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
                var userRole = _httpContextAccessor.HttpContext?.User?.FindFirst("UserType")?.Value;
                UserType userType = (UserType)Enum.Parse(typeof(UserType), userRole);

                var ticket = await _unitOfWork.tickets.GetTicketWithDetailsAsync(ticketId);
                if (ticket != null)
                {
                    if (ticket.Status == TicketStatus.New)
                    {
                        return new ResponseModel<GetCommentDto>
                        {
                            IsSuccess = false,
                            Message = "CannotAddCommentsToANewTicket."
                        };
                    }
                    var comment = _mapper.Map<Comment>(commentDto);
                    comment.CreatedById = userId;
                    comment.UpdateById = userId;
                    comment.TicketId = ticketId;
                    ticket.Comments.Add(comment);
                    await _unitOfWork.tickets.UpdateAsync(ticket);
                    await _unitOfWork.SaveChangesAsync();

                    var commentDtoResult = _mapper.Map<GetCommentDto>(comment);

                    if (userType == UserType.Client)
                    {
                        var supportUser = await _unitOfWork.users.GetUserById(ticket.AssignedToId);
                        await _emailSender.SendEmailAsync(
                            supportUser.Email,
                            "New Comment Added To Ticket",
                            $"A new comment has been added to ticket with ticket id : #{ticketId} by the client.");
                    }
                    else if (userType == UserType.Support)
                    {
                        var clientUser = await _unitOfWork.users.GetUserById(ticket.CreatedById);
                        await _emailSender.SendEmailAsync(
                                clientUser.Email,
                                "New Comment Added To Ticket",
                                $"A new comment has been added to your ticket with ticket id : #{ticketId} by the support.");
                    }

                    return new ResponseModel<GetCommentDto>
                    {
                        Result = commentDtoResult,
                        IsSuccess = true,
                        Message = string.Empty
                    };
                }
                return new ResponseModel<GetCommentDto>
                {
                    IsSuccess = false,
                    Message = "TicketNotFound"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a comment to the ticket.");
                return new ResponseModel<GetCommentDto>
                {
                    IsSuccess = false,
                    Message = "ErrorFound"
                };
            }
        }

        public async Task<ResponseModel<IEnumerable<GetTicketDto>>> GetAllTicketsAsync(int pageNumber, int pageSize, string status = null)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User?.FindFirst("UserId")?.Value;
                var user = await _unitOfWork.users.GetUserById(userId);
                var userType = user.UserType;

                IEnumerable<Ticket> tickets = await _unitOfWork.tickets
                    .GetAllTicketsWithDetailsAsync();

                if (!string.IsNullOrEmpty(status) && Enum.TryParse(status, out TicketStatus statusEnum))
                {
                    tickets = tickets.Where(t => t.Status == statusEnum);
                }

                switch (userType)
                {
                    case UserType.Manager:
                        break;
                    case UserType.Support:
                        tickets = tickets.Where(t => t.AssignedToId == userId);
                        break;
                    case UserType.Client:
                        tickets = tickets.Where(t => t.CreatedById == userId);
                        break;
                }

                var pagedTickets = tickets
                   .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize);

                var pagedTicketsList = pagedTickets.ToList();
                var ticketDtos = _mapper.Map<IEnumerable<GetTicketDto>>(pagedTicketsList);

                return new ResponseModel<IEnumerable<GetTicketDto>>
                {
                    Result = ticketDtos,
                    IsSuccess = true,
                    Message = string.Empty
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all tickets.");
                return new ResponseModel<IEnumerable<GetTicketDto>>
                {
                    IsSuccess = false,
                    Message = "ErrorFound"
                };
            }
        }


        public async Task<ResponseModel<bool>> UpdateTicketCommentAsync(int ticketId, int commentId, string newContent)
        {
            try
            {
                if (string.IsNullOrEmpty(newContent))
                {
                    return new ResponseModel<bool>
                    {
                        IsSuccess = false,
                        Message = "EmptyContent",
                        Result = false
                    };
                }

                var ticket = await _unitOfWork.tickets.GetTicketWithDetailsAsync(ticketId);

                if (ticket != null)
                {
                    if (ticket.Status == TicketStatus.Closed || ticket.Status == TicketStatus.Canceled)
                    {
                        return new ResponseModel<bool>
                        {
                            IsSuccess = false,
                            Message = "CannotUpdateCommentOnATicketThatIsAlreadyClosedOrCanceled",
                            Result = false
                        };
                    }
                    var comment = ticket.Comments.FirstOrDefault(c => c.CommentId == commentId);
                    if (comment != null)
                    {
                        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
                        var userRole = _httpContextAccessor.HttpContext?.User?.FindFirst("UserType")?.Value;
                        UserType userType = (UserType)Enum.Parse(typeof(UserType), userRole);

                        comment.Text = newContent;
                        comment.UpdateById = userId;
                        comment.UpdateDate = DateTime.UtcNow;

                        await _unitOfWork.tickets.UpdateAsync(ticket);
                        await _unitOfWork.SaveChangesAsync();

                        if (userType == UserType.Client)
                        {
                            var supportUser = await _unitOfWork.users.GetUserById(ticket.AssignedToId);
                            if (supportUser != null)
                            {
                                await _emailSender.SendEmailAsync(
                                    supportUser.Email,
                                    "Comment Updated on Ticket",
                                    $"A comment on ticket #{ticketId} has been updated by the client."
                                );
                            }
                        }
                        else if (userType == UserType.Support)
                        {
                            var clientUser = await _unitOfWork.users.GetUserById(ticket.CreatedById);
                            await _emailSender.SendEmailAsync(
                                    clientUser.Email,
                                    "Comment Updated on Ticket",
                                    $"A comment on ticket #{ticketId} has been updated by the support team."
                                );
                        }
                        return new ResponseModel<bool>
                        {
                            IsSuccess = true,
                            Message = string.Empty,
                            Result = true
                        };
                    }
                    return new ResponseModel<bool>
                    {
                        IsSuccess = false,
                        Message = "CommentNotFound",
                        Result = false
                    };
                }
                return new ResponseModel<bool>
                {
                    IsSuccess = false,
                    Message = "TicketNotFound",
                    Result = false
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the ticket comment.");
                return new ResponseModel<bool>
                {
                    IsSuccess = false,
                    Message = "ErrorFound",
                    Result = false
                };
            }
        }

        public async Task<ResponseModel<bool>> DeleteTicketCommentAsync(int ticketId, int commentId)
        {
            try
            {
                var ticket = await _unitOfWork.tickets.GetTicketWithDetailsAsync(ticketId);
                if (ticket != null)
                {
                    if (ticket.Status == TicketStatus.Closed || ticket.Status == TicketStatus.Canceled)
                    {
                        return new ResponseModel<bool>
                        {
                            IsSuccess = false,
                            Message = "CannotDeleteACommentOnATicketThatIsAlreadyClosedOrCanceled.",
                            Result = false
                        };
                    }
                    var comment = ticket.Comments.FirstOrDefault(c => c.CommentId == commentId);
                    if (comment != null)
                    {
                        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
                        var userRole = _httpContextAccessor.HttpContext?.User?.FindFirst("UserType")?.Value;
                        UserType userType = (UserType)Enum.Parse(typeof(UserType), userRole);

                        ticket.Comments.Remove(comment);
                        await _unitOfWork.tickets.UpdateAsync(ticket);
                        await _unitOfWork.SaveChangesAsync();

                        if (userType == UserType.Client)
                        {
                            var supportUser = await _unitOfWork.users.GetUserById(ticket.AssignedToId);
                            await _emailSender.SendEmailAsync(
                                supportUser.Email,
                                "Comment Deleted from Ticket",
                                $"A comment on ticket #{ticketId} has been deleted by the client."
                            );
                        }
                        else if (userType == UserType.Support)
                        {
                            var clientUser = await _unitOfWork.users.GetUserById(ticket.CreatedById);
                            await _emailSender.SendEmailAsync(
                                clientUser.Email,
                                "Comment Deleted from Your Ticket",
                                $"A comment on your ticket #{ticketId} has been deleted by the support team."
                            );
                        }

                        return new ResponseModel<bool>
                        {
                            IsSuccess = true,
                            Message = string.Empty,
                            Result = true
                        };
                    }
                    return new ResponseModel<bool>
                    {
                        IsSuccess = false,
                        Message = "CommentNotFound",
                        Result = false
                    };
                }
                return new ResponseModel<bool>
                {
                    IsSuccess = false,
                    Message = "TicketNotFound",
                    Result = false
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the ticket comment.");
                return new ResponseModel<bool>
                {
                    IsSuccess = false,
                    Message = "ErrorFound",
                    Result = false
                };
            }
        }


        public async Task<ResponseModel<TicketDto>> CreateTicketAsync(PostTicketDto ticketDto)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User?.FindFirst("UserId")?.Value;
                var ticket = _mapper.Map<Ticket>(ticketDto);
                ticket.CreatedById = userId;
                ticket.UpdateById = userId;
                ticket.Status = TicketStatus.New;
                ticket.CreatedDate = DateTime.UtcNow;
                ticket.UpdateDate = DateTime.UtcNow;

                await _unitOfWork.tickets.AddAsync(ticket);
                await _unitOfWork.SaveChangesAsync();

                if (ticketDto.Attachments != null && ticketDto.Attachments.Any())
                {
                    foreach (var img in ticketDto.Attachments)
                    {
                        var imageBytes = Convert.FromBase64String(img);
                        var uniqueFileName = $"{Guid.NewGuid()}.jpg";
                        var imagePath = Path.Combine("wwwroot", "images", uniqueFileName);
                        await System.IO.File.WriteAllBytesAsync(imagePath, imageBytes);
                        var ticketAttachment = new TicketAttachment
                        {
                            FilePath = imagePath,
                            FileName = uniqueFileName,
                            CreatedById = userId,
                            UpdateById = userId,
                            TicketId = ticket.TicketId
                        };
                        ticket.Attachments.Add(ticketAttachment);
                    }

                    await _unitOfWork.tickets.UpdateAsync(ticket);
                    await _unitOfWork.SaveChangesAsync();
                }

                var savedTicket = await _unitOfWork.tickets.GetAll()
                    .Include(t => t.CreatedBy)
                    .Include(t => t.Attachments)
                    .FirstOrDefaultAsync(t => t.TicketId == ticket.TicketId);

                var managers = await _unitOfWork.users.GetAllUsers(u => u.UserType == UserType.Manager);
                var manager = managers.FirstOrDefault();
                await _emailSender.SendEmailAsync(
                    manager.Email,
                    "New Ticket Created",
                    $"A new ticket #{ticket.TicketId} has been created by {ticket.CreatedBy.UserName}."
                );

                var ticketDtoResult = _mapper.Map<TicketDto>(savedTicket);
                ticketDtoResult.CreatedBy = savedTicket.UpdateBy.UserName;
                ticketDtoResult.Attachments = savedTicket.Attachments.Select(a => new TicketAttachmentDto
                {
                    FileName = a.FileName,
                }).ToList();

                return new ResponseModel<TicketDto>
                {
                    Result = ticketDtoResult,
                    IsSuccess = true,
                    Message = string.Empty
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the ticket.");
                return new ResponseModel<TicketDto>
                {
                    IsSuccess = false,
                    Message = "ErrorFound"
                };
            }
        }

        public async Task<ResponseModel<IEnumerable<GetTicketDto>>> GetAllTicketsByUserIdAsync(string userId)
        {
            try
            {
                var user = await _unitOfWork.users.GetUserById(userId);
                if (user == null)
                {
                    return new ResponseModel<IEnumerable<GetTicketDto>>
                    {
                        IsSuccess = false,
                        Message = "UserNotFound"
                    };
                }

                IEnumerable<Ticket> tickets = Enumerable.Empty<Ticket>();
                var TuserId = _httpContextAccessor.HttpContext.User?.FindFirst("UserId")?.Value;
                var userRole = _httpContextAccessor.HttpContext.User?.FindFirst("UserType")?.Value;
                UserType userType = (UserType)Enum.Parse(typeof(UserType), userRole);

                if (userType == UserType.Client && user.UserId != TuserId)
                {
                    return new ResponseModel<IEnumerable<GetTicketDto>>
                    {
                        IsSuccess = false,
                        Message = "NotAuthorized"
                    };
                }
                switch (user.UserType)
                {
                    case UserType.Manager:
                        tickets = await _unitOfWork.tickets.GetAllTicketsWithDetailsAsync();
                        break;
                    case UserType.Support:
                        tickets = await _unitOfWork.tickets.GetAllTicketsWithDetailsAsync();
                        tickets = tickets.Where(t => t.AssignedToId == userId);
                        break;
                    case UserType.Client:
                        tickets = await _unitOfWork.tickets.GetAllTicketsWithDetailsAsync();
                        tickets = tickets.Where(t => t.CreatedById == userId);
                        break;
                }

                var ticketDtos = new List<GetTicketDto>();
                foreach (var ticket in tickets)
                {
                    var ticketDto = _mapper.Map<GetTicketDto>(ticket);
                    ticketDto.Attachments = new List<string>();
                    foreach (var attachment in ticket.Attachments)
                    {
                        if (File.Exists(attachment.FilePath))
                        {
                            byte[] imageBytes = await File.ReadAllBytesAsync(attachment.FilePath);
                            var base64 = Convert.ToBase64String(imageBytes);
                            ticketDto.Attachments.Add(base64);
                        }
                    }
                    ticketDtos.Add(ticketDto);
                }

                return new ResponseModel<IEnumerable<GetTicketDto>>
                {
                    Result = ticketDtos,
                    IsSuccess = true,
                    Message = string.Empty
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all tickets.");
                return new ResponseModel<IEnumerable<GetTicketDto>>
                {
                    IsSuccess = false,
                    Message = "ErrorFound"
                };
            }
        }

        public async Task<ResponseModel<bool>> UpdateTicketAsync(PutTicketDto updateTicketDto)
        {
            try
            {
                var ticket = await _unitOfWork.tickets.GetTicketWithDetailsAsync(updateTicketDto.TicketId);
                if (ticket == null)
                {
                    return new ResponseModel<bool>
                    {
                        IsSuccess = false,
                        Message = "TicketNotFound",
                        Result = false
                    };
                }
                if (ticket.Status == TicketStatus.Closed || ticket.Status == TicketStatus.Canceled)
                {
                    return new ResponseModel<bool>
                    {
                        IsSuccess = false,
                        Message = "CannotUpdateATicketThatIsAlreadyClosedOrCanceled.",
                        Result = false
                    };
                }
                var userId = _httpContextAccessor.HttpContext.User?.FindFirst("UserId")?.Value;
                var userRole = _httpContextAccessor.HttpContext.User?.FindFirst("UserType")?.Value;
                UserType userType = (UserType)Enum.Parse(typeof(UserType), userRole);

                if (ticket.CreatedById != userId && ticket.AssignedToId != userId)
                {
                    return new ResponseModel<bool>
                    {
                        IsSuccess = false,
                        Message = "Unauthorized",
                        Result = false
                    };
                }

                if (ticket.Status == TicketStatus.New || ticket.AssignedToId == null)
                {
                    return new ResponseModel<bool>
                    {
                        IsSuccess = false,
                        Message = "CannotUpdateANewOrUnassignedTicket.",
                        Result = false
                    };
                }

                ticket.ProblemDescription = updateTicketDto.ProblemDescription;
                ticket.ProductId = updateTicketDto.ProductId;
                ticket.TicketTypeId = updateTicketDto.TicketTypeId;
                ticket.UpdateDate = DateTime.UtcNow;
                ticket.UpdateById = userId;

                if (updateTicketDto.Attachments != null && updateTicketDto.Attachments.Any())
                {
                    var attachmentsToRemove = ticket.Attachments.ToList();
                    foreach (var attachment in attachmentsToRemove)
                    {
                        if (File.Exists(attachment.FilePath))
                        {
                            File.Delete(attachment.FilePath);
                        }
                        var res = await _unitOfWork.ticketAttachments.Delete(attachment.AttachmentId);
                        Console.WriteLine(res == true ? "true" : "false");
                    }
                    ticket.Attachments.Clear();
                    await _unitOfWork.SaveChangesAsync();
                    foreach (var base64String in updateTicketDto.Attachments)
                    {
                        var imageBytes = Convert.FromBase64String(base64String);
                        var uniqueFileName = $"{Guid.NewGuid()}.jpg";
                        var filePath = Path.Combine("wwwroot", "attachments", uniqueFileName);
                        await File.WriteAllBytesAsync(filePath, imageBytes);
                        var ticketAttachment = new TicketAttachment
                        {
                            FilePath = filePath,
                            FileName = uniqueFileName,
                            CreatedById = userId,
                            UpdateById = userId,
                            TicketId = ticket.TicketId,
                            CreatedDate = DateTime.UtcNow,
                            UpdateDate = DateTime.UtcNow
                        };

                        ticket.Attachments.Add(ticketAttachment);
                    }
                }

                var ticketDetail = new TicketDetail
                {
                    TicketId = ticket.TicketId,
                    CreateById = userId,
                    Status = ticket.Status.ToString(),
                    CreatedDate = DateTime.UtcNow
                };

                await _unitOfWork.ticketDetails.AddAsync(ticketDetail);
                await _unitOfWork.tickets.UpdateAsync(ticket);
                await _unitOfWork.SaveChangesAsync();

                if (userType == UserType.Client)
                {
                    var supportUser = await _unitOfWork.users.GetUserById(ticket.AssignedToId);
                    await _emailSender.SendEmailAsync(
                        supportUser.Email,
                        "Ticket Has Been Updated",
                        $"The ticket #{ticket.TicketId} has been updated by the client."
                    );
                }
                else if (userType == UserType.Support)
                {
                    var clientUser = await _unitOfWork.users.GetUserById(ticket.CreatedById);
                    await _emailSender.SendEmailAsync(
                        clientUser.Email,
                        "Ticket Has Been Updated",
                        $"Your ticket #{ticket.TicketId} has been updated by support."
                    );
                }

                return new ResponseModel<bool>
                {
                    IsSuccess = true,
                    Message = string.Empty,
                    Result = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the ticket.");
                return new ResponseModel<bool>
                {
                    IsSuccess = false,
                    Message = "ErrorFound",
                    Result = false
                };
            }
        }

        public async Task<ResponseModel<List<LookUpDataModel<int>>>> GetTicketLookup()
        {
            try
            {
                var result = Enum.GetValues(typeof(TicketStatus))
                             .Cast<TicketStatus>()
                             .Select(enumValue => new LookUpDataModel<int>
                             {
                                 Value = Convert.ToInt32(enumValue),
                                 NameAr = enumValue.ToString(),
                                 NameEn = enumValue.ToString()
                             }).ToList();
                return new ResponseModel<List<LookUpDataModel<int>>> { Result = result, IsSuccess = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the ticket statuses.");
                return new ResponseModel<List<LookUpDataModel<int>>>
                {
                    Result = new List<LookUpDataModel<int>>(),
                    IsSuccess = false,
                    Message = "ErrorFound"
                };
            }
        }
    }
}
