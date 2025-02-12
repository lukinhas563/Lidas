﻿using Lidas.MangaApi.Entities;
using Lidas.MangaApi.Models.PageModels;

namespace Lidas.MangaApi.Models.ViewModels;

public class MangaView
{
    public Guid Id { get; set; }
    public string Banner { get; set; }
    public string Cover { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Release { get; set; }
    public int Likes { get; set; }
    public List<CategoryViewList> Categories { get; set; }
    public List<AuthorViewList> Authors { get; set; }
    public PageView<ChapterView> Chapters { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
