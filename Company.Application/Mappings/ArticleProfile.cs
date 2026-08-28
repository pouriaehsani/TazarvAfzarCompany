using AutoMapper;
using Company.Application.DTOs.Article;
using Company.Domain.Entities;

namespace Company.Application.Mappings;

public class ArticleProfile : Profile
{
    public ArticleProfile() 
    {
        CreateMap<CreateArticleDto, Article>()
            .ForMember(
                dest => dest.ImagePath,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.ArticleTags,
                opt => opt.Ignore());
        CreateMap<UpdateArticleDto, Article>();
    }
}