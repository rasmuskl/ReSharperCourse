using System;
using System.Collections.Generic;

namespace Basic.InlineCompletion
{
    public class VariablesCompletion
    {
        /// <summary>
        /// Gets the item range for a given page.
        /// </summary>
        public int[] IntroduceVariable(int page, int numberOfPages, int itemsPerPage)
        {
            return new[]
                {
                    Math.Min(Math.Max(page, 0), numberOfPages - 1) * itemsPerPage, 
                    ((Math.Min(Math.Max(page, 0), numberOfPages - 1) + 1) * itemsPerPage) - 1
                };
        }

        /// <summary>
        /// Gets the item range for a given page.
        /// </summary>
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

        public void ImportCompletion(List<string> list)
        {

            Console.Out.WriteLine(list);
        }

        public void IntroduceVariableImportCompletionCombo()
        {
        }
    }
}