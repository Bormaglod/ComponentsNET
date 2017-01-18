/*
 * Created by SharpDevelop.
 * User: Сергей
 * Date: 19.11.2006
 * Time: 20:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;

namespace ComponentsNET
{
	public class cnTabPageCollection : CollectionBase
	{
		private cnTabControl tabControl;
		
		public cnTabPageCollection(cnTabControl t)
		{
			tabControl = t;
		}
		
		public cnTabPage this[int index]
		{
			get { return (base.List[index] as cnTabPage); }
		}
		
		public cnTabPage Add(cnTabPage value)
		{
			SetUpPage(value);
			base.List.Add(value as object);
			tabControl.Controls.Add(value);
			return value;
		}
		
		public void AddRange(cnTabPage[] values)
		{
			foreach(cnTabPage page in values)
			{
				SetUpPage(page);
				tabControl.Controls.Add(page);
				Add(page);
			}
		}

		public void Remove(cnTabPage value)
		{
			tabControl.Controls.Remove(value);
			base.List.Remove(value as object);
		}

		public void Insert(int index, cnTabPage value)
		{
			SetUpPage(value);
			tabControl.Controls.Add(value);
			base.List.Insert(index, value as object);
		}

		public bool Contains(cnTabPage value)
		{
			return base.List.Contains(value as object);
		}
		
		public int IndexOf(cnTabPage value)
		{
			return base.List.IndexOf(value);
		}
		
		public void UpdatePagesSettings()
		{
			foreach(cnTabPage page in this)
				SetUpPage(page);
		}
		
		private void SetUpPage(cnTabPage page)
		{
			page.SuspendLayout();

			page.Visible = false;
			page.Size = tabControl.Size;

			if( tabControl.Alignment == TabAlignment.Left || tabControl.Alignment == TabAlignment.Right )
				page.Size = new Size(page.Size.Width - tabControl.TabHeight - 1, page.Size.Height - 2);
			else
				page.Size = new Size(page.Size.Width - 2, page.Size.Height - tabControl.TabHeight - 1);

			switch(tabControl.Alignment)
			{
				case TabAlignment.Left:
					page.Location = new Point(tabControl.TabHeight, 1);
					break;
				case TabAlignment.Top:
					page.Location = new Point(1, tabControl.TabHeight);
					break;
				default:
					page.Location = new Point(1, 1);
					break;
			}

			page.ResumeLayout(false);
		}
		
		public delegate void ClearCompleteEventHandler();
		public delegate void InsertCompleteEventHandler(int index, cnTabPage value);
		public delegate void RemoveCompleteEventHandler(int index, cnTabPage value);
		public delegate void SetCompleteEventHandler(int index, cnTabPage oldValue, cnTabPage newValue);

		public event ClearCompleteEventHandler ClearComplete;
		public event InsertCompleteEventHandler InsertComplete;
		public event RemoveCompleteEventHandler RemoveComplete;
		public event SetCompleteEventHandler SetComplete;

		protected override void OnClearComplete()
		{
			if(ClearComplete != null)
				ClearComplete();
		}

		protected override void OnInsertComplete(int index, object value)
		{
			if(InsertComplete != null)
				InsertComplete(index, value as cnTabPage);
		}

		protected override void OnRemoveComplete(int index, object value)
		{
			if(RemoveComplete != null)
				RemoveComplete(index, value as cnTabPage);
		}

		protected override void OnSetComplete(int index, object oldValue, object newValue)
		{
			if(SetComplete != null)
				SetComplete(index, oldValue as cnTabPage, newValue as cnTabPage);
		}
	}
}
