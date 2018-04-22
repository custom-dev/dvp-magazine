using System;
using System.Collections.Generic;
using System.Text;

namespace Developpez.MagazineTool
{
    partial class MagazineType
    {
        public ArticleType GetArticleByID(string articleID)
        {
            foreach(MagazineTypeCategorie categorie in this.Categories)
            {
                foreach(ArticleType article in categorie.Articles)
                {
                    if (article.ID == articleID)
                    {
                        return article;
                    }
                }
            }

            return null;
        }
    }
}
