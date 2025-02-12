﻿using Lidas.MangaApi.Entities;

namespace Lidas.MangaApi.Models.ViewModels;

public class MangaViewList
{
    public Guid Id { get; set; }
    public string Banner { get; set; }
    public string Cover { get; set; }
    public string Name { get; set; }
    public List<CategoryViewList> Categories { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
