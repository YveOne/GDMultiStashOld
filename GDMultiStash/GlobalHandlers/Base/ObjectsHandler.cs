﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using GDMultiStash.Common.Objects;

namespace GDMultiStash.GlobalHandlers.Base
{
    internal class ObjectsHandler<T> : BaseHandler where T: IBaseObject
    {

        protected Dictionary<int, T> Items { get; private set; }

        public ObjectsHandler() : base()
        {
            Items = new Dictionary<int, T>();
        }

        public void ResetOrder(IList<T> list)
        {
            List<int> orders = new List<int>();
            foreach (T item in list)
            {
                orders.Add(item.Order);
            }
            orders.Sort();
            foreach (T item in list)
            {
                item.Order = orders[0];
                orders.RemoveAt(0);
            }
        }

    }
}
