using Brainbay.Submission.DataAccess.Models.Domain;
using Brainbay.Submission.DataAccess.Models.Dto;
using System;
using System.Collections.Generic;

namespace RickAndMorty.Net.Api.Service
{
    public class PagedCharacters
    {
        public PageInfoDto PageInfo;
        public IEnumerable<Character> Characters;

        public int CurrentPage;

        public PagedCharacters(int currentPage,PageInfoDto pageInfo, IEnumerable<Character> characters)
        {
            CurrentPage = currentPage;
            this.PageInfo = pageInfo;
            this.Characters = characters;
        }

        public override bool Equals(object obj) => obj is PagedCharacters other 
            && CurrentPage == other.CurrentPage
            && EqualityComparer<PageInfoDto>.Default.Equals(this.PageInfo, other.PageInfo)
            && EqualityComparer<IEnumerable<Character>>.Default.Equals(this.Characters, other.Characters);
        public override int GetHashCode() => HashCode.Combine(this.CurrentPage, this.PageInfo, this.Characters);
         

    }
}
