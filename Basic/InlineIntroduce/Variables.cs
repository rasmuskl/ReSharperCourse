using System;

namespace Basic.InlineIntroduce
{
    public class Variables
    {
        public int[] IntroduceVariable(int page, int numberOfPages, int itemsPerPage)
        {
            return new[]
                {
                    Math.Min(Math.Max(page, 0), numberOfPages - 1) * itemsPerPage, 
                    ((Math.Min(Math.Max(page, 0), numberOfPages - 1) + 1) * itemsPerPage) - 1
                };
        }

        public int[] InlineVariable(int page, int numberOfPages, int itemsPerPage)
        {
            int firstPage = 0;
            int lastPage = numberOfPages - 1;
            int currentPage = Math.Min(Math.Max(page, firstPage), lastPage);

            int firstItemInPage = currentPage * itemsPerPage;
            int lastItemInPage = ((currentPage + 1) * itemsPerPage) - 1;

            var itemRange = new[] { firstItemInPage, lastItemInPage };

            return itemRange;
        }
    }
}