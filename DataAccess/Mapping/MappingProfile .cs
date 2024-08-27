using AutoMapper;
using System.Net.Mail;
using System.Numerics;
using System.Xml.Linq;
using System;
using TicketSystem.DataAccess.DTOs;
using TicketSystem.DataAccess.Models;

namespace TicketSystem.DataAccess.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Ticket, TicketDto>()
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy.FullName))
                .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.UpdateBy.FullName))
                .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.Attachments))
                .ReverseMap();

            CreateMap<Ticket, GetTicketDto>()
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy.FullName))
                .ForMember(dest => dest.AssignedTo, opt => opt.MapFrom(src => src.AssignedTo != null ? src.AssignedTo.FullName : null))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.TicketCategoryAr, opt => opt.MapFrom(src => src.TicketType.TicketCategory.NameAr))
                .ForMember(dest => dest.TicketCategoryEn, opt => opt.MapFrom(src => src.TicketType.TicketCategory.NameEn))
                .ForMember(dest => dest.TicketTypeAr, opt => opt.MapFrom(src => src.TicketType.NameAr))
                .ForMember(dest => dest.TicketTypeEn, opt => opt.MapFrom(src => src.TicketType.NameEn))
                .ForMember(dest => dest.ProductNameAr, opt => opt.MapFrom(src => src.Product.NameAr))
                .ForMember(dest => dest.ProductNameEn, opt => opt.MapFrom(src => src.Product.NameEn))
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments.Select(c => new GetCommentDto
                {
                    CommentId = c.CommentId,
                    Text = c.Text,
                    CreatedBy = c.CreatedBy.FullName,
                    CreatedDate = c.CreatedDate,
                    UpdateDate = c.UpdateDate
                }).ToList()))
                .ForMember(dest => dest.TicketDetails, opt => opt.MapFrom(src => src.TicketDetails.Select(d => new TicketDetailDto
                {
                    Status = d.Status,
                    CreatedBy = d.CreateBy.FullName,
                    CreatedDate = d.CreatedDate
                }).ToList()))
                .ReverseMap();


            CreateMap<Ticket, GetOneTicketDto>()
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy.FullName))
            .ForMember(dest => dest.AssignedTo, opt => opt.MapFrom(src => src.AssignedTo != null ? src.AssignedTo.FullName : null))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.TicketCategoryAr, opt => opt.MapFrom(src => src.TicketType.TicketCategory.NameAr))
            .ForMember(dest => dest.TicketCategoryEn, opt => opt.MapFrom(src => src.TicketType.TicketCategory.NameEn))
            .ForMember(dest => dest.TicketTypeAr, opt => opt.MapFrom(src => src.TicketType.NameAr))
            .ForMember(dest => dest.TicketTypeEn, opt => opt.MapFrom(src => src.TicketType.NameEn))
            .ForMember(dest => dest.ProductNameAr, opt => opt.MapFrom(src => src.Product.NameAr))
            .ForMember(dest => dest.ProductNameEn, opt => opt.MapFrom(src => src.Product.NameEn))
            .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments.Select(c => new GetCommentDto
            {
                CommentId = c.CommentId,
                Text = c.Text,
                CreatedBy = c.CreatedBy.FullName,
                CreatedDate = c.CreatedDate,
                UpdateDate = c.UpdateDate
            }).ToList()))
            .ForMember(dest => dest.TicketDetails, opt => opt.MapFrom(src => src.TicketDetails.Select(d => new TicketDetailDto
            {
                Status = d.Status,
                CreatedBy = d.CreateBy.FullName,
                CreatedDate = d.CreatedDate
            }).ToList()))
    .ReverseMap();

            CreateMap<AppUser, GetAllUsersWithTicketsDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName != null ? src.FullName : "N/A"))
                .ForMember(dest => dest.Tickets, opt => opt.MapFrom(src => src.CreatedTickets.Select(ticket => new GetTicketDto
                {
                    TicketId = ticket.TicketId,
                    CreatedBy = ticket.CreatedBy != null ? ticket.CreatedBy.FullName : "N/A",
                    AssignedTo = ticket.AssignedTo != null ? ticket.AssignedTo.FullName : "N/A",
                    ProblemDescription = ticket.ProblemDescription,
                    Status = ticket.Status.ToString(),
                    CreatedDate = ticket.CreatedDate,
                    UpdateDate = ticket.UpdateDate,
                    Comments = ticket.Comments.Select(comment => new GetCommentDto
                    {
                        CommentId = comment.CommentId,
                        Text = comment.Text,
                        CreatedBy = comment.CreatedBy != null ? comment.CreatedBy.FullName : "N/A",
                        CreatedDate = comment.CreatedDate,
                        UpdateDate = comment.UpdateDate
                    }).ToList(),
                    TicketDetails = ticket.TicketDetails.Select(detail => new TicketDetailDto
                    {
                        Status = detail.Status.ToString(),
                        CreatedBy = detail.CreateBy != null ? detail.CreateBy.FullName : "N/A",
                        CreatedDate = detail.CreatedDate
                    }).ToList(),
                    TicketCategoryAr = ticket.TicketType.TicketCategory != null ? ticket.TicketType.TicketCategory.NameAr : "N/A",
                    TicketCategoryEn = ticket.TicketType.TicketCategory != null ? ticket.TicketType.TicketCategory.NameEn : "N/A",
                    TicketTypeAr = ticket.TicketType != null ? ticket.TicketType.NameAr : "N/A",
                    TicketTypeEn = ticket.TicketType != null ? ticket.TicketType.NameEn : "N/A",
                    ProductNameAr = ticket.Product != null ? ticket.Product.NameAr : "N/A",
                    ProductNameEn = ticket.Product != null ? ticket.Product.NameEn : "N/A",
                }).ToList()))
                    .ReverseMap();

            CreateMap<Ticket, PostTicketDto>().ReverseMap();
            CreateMap<Comment, GetCommentDto>()
                .ForMember(dest => dest.CommentId, opt => opt.MapFrom(src => src.CommentId))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy.FullName))
                .ReverseMap();
            CreateMap<TicketDetail, TicketDetailDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreateBy.FullName))
                .ReverseMap();
            CreateMap<Comment, PostCommentDto>().ReverseMap();
            CreateMap<TicketCategoryDto, TicketCategory>().ReverseMap();
            CreateMap<PostTicketCategoryDto, TicketCategory>();
            CreateMap<TicketType, TicketTypeDto>().ReverseMap();
            CreateMap<TicketType, PostTicketTypeDto>().ReverseMap();
            CreateMap<TicketAttachment, TicketAttachmentDto>().ReverseMap();

            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Product, PostProductDto>().ReverseMap();

            CreateMap<AppUser, UserWithTicketStatsDto>().ReverseMap();
            CreateMap<AppUser, GetUserDto>()
                .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => src.UserType.ToString()));

            CreateMap<AppUser, UserDto>().ReverseMap();
            CreateMap<AppUser, PutUserDto>().ReverseMap();
            CreateMap<AppUser, PostUserDto>().ReverseMap();
            CreateMap<AppUser, PutUserDtoManger>().ReverseMap();
            CreateMap<AppUser, GetOneUserDto>().ReverseMap();

            CreateMap<TicketType, LookUpDataModel<int>>()
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.TicketTypeId))
            .ForMember(dest => dest.NameAr, opt => opt.MapFrom(src => src.NameAr))
            .ForMember(dest => dest.NameEn, opt => opt.MapFrom(src => src.NameEn)); ;


            CreateMap<Product, LookUpDataModel<int>>()
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.NameAr, opt => opt.MapFrom(src => src.NameAr))
            .ForMember(dest => dest.NameEn, opt => opt.MapFrom(src => src.NameEn));

            CreateMap<TicketCategory, LookUpDataModel<int>>()
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.TicketCategoryId))
            .ForMember(dest => dest.NameAr, opt => opt.MapFrom(src => src.NameAr))
            .ForMember(dest => dest.NameEn, opt => opt.MapFrom(src => src.NameEn));

            CreateMap<PostTicketDto, Ticket>()
            .ForMember(dest => dest.Attachments, opt => opt.Ignore());
            CreateMap<Ticket, TicketDto>()
            .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.Attachments)).ReverseMap();

        }
    }
}
