/*
 * Created by SharpDevelop.
 * User: Сергей
 * Date: 21.11.2006
 * Time: 1:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace ComponentsNET
{
	public class cnTabControlDesigner : System.Windows.Forms.Design.ParentControlDesigner
	{
		private DesignerVerb verbRemovePage;
		private IDesignerHost host = null;
		private ISelectionService selService = null;
		
		private cnTabControl TabControl
		{
			get
			{
				return this.Control as cnTabControl;
			}
		}
		
		private IDesignerHost DesignerHost
		{
			get
			{
				if( host == null )
					host = (IDesignerHost)GetService(typeof(IDesignerHost));

				return host;
			}
		}
		
		public ISelectionService SelectionService
		{
			get
			{
				if(selService == null)
					selService = (ISelectionService)GetService(typeof(ISelectionService));

				return selService;
			}
		}
		
		public override DesignerVerbCollection Verbs
		{
			get
			{
				DesignerVerbCollection v = new DesignerVerbCollection();

				v.Add(new DesignerVerb("&Add Page", new EventHandler(AddPage)) );
				v.Add(verbRemovePage = new DesignerVerb("&Remove Page", new EventHandler(RemovePage)));

				verbRemovePage.Enabled = TabControl.TabPages.Count > 0;

				return v;
			}
		}
		
		private void AddPage(object sender, System.EventArgs e)
		{
			if (DesignerHost != null)
			{
				IComponent c = DesignerHost.CreateComponent(typeof(ComponentsNET.cnTabPage));
				TabControl.TabPages.Add(c as cnTabPage);
				verbRemovePage.Enabled = true;
			}
		}

		private void RemovePage(object sender, System.EventArgs e)
		{
			if (DesignerHost != null)
			{
				DesignerHost.DestroyComponent(TabControl.TabPages[TabControl.SelectedIndex]);
				TabControl.TabPages.RemoveAt(TabControl.SelectedIndex);
				verbRemovePage.Enabled = TabControl.TabPages.Count > 0;
			}
		}
		
		protected override bool GetHitTest(Point p)
		{
			Point cp = TabControl.PointToClient(p);

			return TabControl.GetHitTest(cp.X, cp.Y) >= 0;
		}

		private void CurrentPageChanged(cnTabControl sender, cnTabPage page)
		{
			if (SelectionService != null)
			{
				System.Collections.ArrayList s = new System.Collections.ArrayList();
				s.Add(TabControl.TabPages[TabControl.SelectedIndex]);
				SelectionService.SetSelectedComponents(s);
			}
		}
		
		public override void Initialize(IComponent component)
		{
			base.Initialize(component);

			this.DrawGrid = false;

			if (TabControl != null)
				TabControl.SelectPage += new cnTabControl.SelectEventHandler(CurrentPageChanged);
		}

		protected override void Dispose(bool disposing)
		{
			if( disposing )
			{
				TabControl.SelectPage -= new cnTabControl.SelectEventHandler(CurrentPageChanged);
			}

			base.Dispose( disposing );
		}
	}
	
	public class cnTabPageDesigner : System.Windows.Forms.Design.ParentControlDesigner
	{
		public override void Initialize(IComponent component)
		{
			base.Initialize(component);
			this.DrawGrid = true;
		}

		public override System.Windows.Forms.Design.SelectionRules SelectionRules
		{
			get
			{
				return System.Windows.Forms.Design.SelectionRules.Locked;
			}
		}
	}
}
