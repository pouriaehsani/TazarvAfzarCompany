using AutoMapper;
using Company.Application.DTOs.Article;
using Company.Domain.Entities;

namespace Company.Application.Mappings;

public class ArticleProfile : Profile
{
    public ArticleProfile() 
    {
        // UpdateArticleDto -> Article is intentionally NOT mapped: the service
        // loads the tracked aggregate and copies fields onto it directly (so
        // image/tag handling stays explicit), rather than mapping a detached
        // entity that would overwrite navigation data.
        CreateMap<CreateArticleDto, Article>()
            .ForMember(
                dest => dest.ImagePath,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.ArticleTags,
                opt => opt.Ignore());
    }
}