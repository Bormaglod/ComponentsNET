//-----------------------------------------------------------------------
// <copyright file="NetTabPageCollection.cs" company="Sergey Teplyashin">
//     Copyright (c) 2010-2012 Sergey Teplyashin. All rights reserved.
// </copyright>
// <author>Тепляшин Сергей Васильевич</author>
// <email>sergey-teplyashin@yandex.ru</email>
// <license>
//     This program is free software; you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation; either version 3 of the License, or
//     (at your option) any later version.
//
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
//
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </license>
// <date>28.02.2011</date>
// <time>9:50</time>
// <summary>Defines the NetTabPageCollection class.</summary>
//-----------------------------------------------------------------------

namespace ComponentsNET
{
    #region Using directives
    
    using System;
    using System.Collections;
    using System.Drawing;
    using System.Windows.Forms;
    
    #endregion
    
    public class NetTabPageCollection : CollectionBase
    {
        private NetTabControl owner;
        
        public NetTabPageCollection(NetTabControl owner)
        {
            this.owner = owner;
        }
        
        public event EventHandler<EventArgs> ClearComplete;
        public event EventHandler<NetTabPageEventArgs> InsertComplete;
        public event EventHandler<NetTabPageEventArgs> RemoveComplete;
        public event EventHandler<NetTabPageEventArgs> SetComplete;
        
        public NetTabPage this[int index]
        {
            get { return (base.List[index] as NetTabPage); }
        }
        
        public int Add(NetTabPage page)
        {
            return base.List.Add(page);
        }
        
        public void AddRange(NetTabPage[] pages)
        {
            foreach(NetTabPage page in pages)
            {
                this.Add(page);
            }
        }

        public void Remove(NetTabPage page)
        {
            base.List.Remove(page);
        }

        public void Insert(int index, NetTabPage page)
        {
            base.List.Insert(index, page);
        }

        public bool Contains(NetTabPage page)
        {
            return base.List.Contains(page);
        }
        
        public int IndexOf(NetTabPage page)
        {
            return base.List.IndexOf(page);
        }
        
        protected override void OnClearComplete()
        {
            if (this.ClearComplete != null)
            {
                this.ClearComplete(this, new EventArgs());
            }
        }

        protected override void OnInsertComplete(int index, object value)
        {
            NetTabPage page = value as NetTabPage;
            if (page != null)
            {
                this.SetUpPage(page);
                if (this.InsertComplete != null)
                {
                    this.InsertComplete(this, new NetTabPageEventArgs(index, page));
                }
            }
        }

        protected override void OnRemoveComplete(int index, object value)
        {
            if (this.RemoveComplete != null)
            {
                this.RemoveComplete(this, new NetTabPageEventArgs(index, value as NetTabPage));
            }
        }

        protected override void OnSetComplete(int index, object oldValue, object newValue)
        {
            if (this.SetComplete != null)
            {
                this.SetComplete(this, new NetTabPageEventArgs(index, oldValue as NetTabPage, newValue as NetTabPage));
            }
        }
        
        private void SetUpPage(NetTabPage page)
        {
            page.SuspendLayout();
            page.Visible = false;

            int width = this.owner.Size.Width;
            int height = this.owner.Size.Height;
            if (this.owner.Alignment == TabAlignment.Left || this.owner.Alignment == TabAlignment.Right)
            {
                width -= this.owner.TabHeight - 1;
                height -= 2;
            }
            else
            {
                width -= 2;
                height -= this.owner.TabHeight - 1;
            }
            
            page.Size = new Size(width, height);

            switch (this.owner.Alignment)
            {
                case TabAlignment.Left:
                    page.Location = new Point(this.owner.TabHeight, 1);
                    break;
                case TabAlignment.Top:
                    page.Location = new Point(1, this.owner.TabHeight);
                    break;
                default:
                    page.Location = new Point(1, 1);
                    break;
            }

            page.ResumeLayout(false);
        }
    }
}
