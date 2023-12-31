﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDMultiStash.Common.Objects.Sorting.Handler.Structures
{
    internal abstract class SortingStructureBase
    {
        public abstract void FillSortedList(
            string sortString,
            GDIALib.Parser.Stash.Item item,
            Global.Database.ItemInfo itemInfo);

        public abstract string[] GetSortedListKeys();
        public abstract List<GDIALib.Parser.Stash.StashTab> GetTabsQueue(
            string sortString,
            uint stashWidth,
            uint stashHeight);
    }

    internal abstract class SortingStructure<sortStruct> : SortingStructureBase
    {
        protected readonly SortedDictionary<string, sortStruct> sortedList = new SortedDictionary<string, sortStruct>();
        public override string[] GetSortedListKeys() => sortedList.Keys.ToArray();
    }
}
