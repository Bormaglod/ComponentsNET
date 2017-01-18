/*
 * Created by SharpDevelop.
 * User: Сергей
 * Date: 29.11.2006
 * Time: 19:34
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel.Design.Serialization;
using System.Reflection;

namespace ComponentsNET
{
	/// <summary>
	/// Класс предназначен для представления заливки элементов
	/// интерфейса и состоит из 2х цветов, а также способа заливки
	/// </summary>
	[TypeConverter(typeof(cnColorConverter))]
	[Serializable()]
	public class cnTabColor
	{
		#region private property
		
		private Color color1;
		private Color color2;
		private TabControlFill fill;
		private Rectangle rect;
		private TabAlignment align;
		[NonSerialized()]
		private Brush brush;
		[NonSerialized()]
		private Pen pen;
		[NonSerialized()]
		private Pen borderPen;
		
		#endregion
		
		#region constructors and destructor
		
		public cnTabColor()
		{
			color1 = Color.White;
			color2 = Color.White;
			fill = TabControlFill.Solid;
			brush = null;
			pen = null;
			borderPen = null;
			rect = new Rectangle(0, 0, 100, 100);
			align = TabAlignment.Top;
		}
		
		public cnTabColor(Color c1, Color c2, TabControlFill f)
		{
			color1 = c1;
			color2 = c2;
			fill = f;
			brush = null;
			pen = null;
			borderPen = null;
			rect = new Rectangle(0, 0, 100, 100);
			align = TabAlignment.Top;
		}
		
		~cnTabColor()
		{
			if (brush != null)
				brush.Dispose();
			if (pen != null)
				pen.Dispose();
			if (borderPen != null)
				borderPen.Dispose();
		}		
		
		#endregion
		
		#region delegate and events
		
		public delegate void ColorChangedEventHandler(cnTabColor sender);
		public event ColorChangedEventHandler ColorChanged;
		
		protected void OnColorChanged()
		{
			if (ColorChanged != null)
				ColorChanged(this);
		}
		
		#endregion
		
		#region published property
		
		[DefaultValue(typeof(Color), "White")]
		[NotifyParentProperty(true)]
		public Color Color1
		{
			get { return color1; }
			set
			{
				if (color1 != value)
				{
					color1 = value;
					CreateColors();
					OnColorChanged();
				}
			}
		}
		
		[DefaultValue(typeof(Color), "White")]
		[NotifyParentProperty(true)]
		public Color Color2
		{
			get { return color2; }
			set
			{
				if (color2 != value)
				{
					color2 = value;
					CreateColors();
					OnColorChanged();
				}
			}
		}
		
		[DefaultValue(TabControlFill.Solid)]
		[NotifyParentProperty(true)]
		public TabControlFill Fill
		{
			get { return fill; }
			set
			{
				if (fill != value)
				{
					fill = value;
					CreateColors();
					OnColorChanged();
				}
			}
		}
		
		#endregion
		
		#region public property
		
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public Brush Brush
		{
			get
			{
				if (brush == null)
					CreateColors();
				return brush;
			}
		}
		
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public Pen Pen
		{
			get
			{
				if (pen == null)
					CreateColors();
				return pen;
			}
		}
		
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public Pen BorderPen
		{
			get
			{
				if (borderPen == null)
					CreateColors();
				return borderPen;
			}
		}
		
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public Rectangle BoundsRect
		{
			get { return rect; }
			set
			{
				if (rect != value)
				{
					rect = value;
					CreateColors();
				}
			}
		}
		
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public TabAlignment Alignment
		{
			get { return align; }
			set 
			{
				if (align != value)
				{
					align = value;
					CreateColors();
				}
			}
		}
		
		#endregion
		
		#region public methods
		
		public void Update(Rectangle r, TabAlignment a)
		{
			align = a;
			rect = r;
			CreateColors();
		}
		
		#endregion
		
		#region private methods
		
		private void CreateColors()
		{
			if (brush != null)
				brush.Dispose();
			if (pen != null)
				pen.Dispose();
			pen = new Pen(brush = GetBrush());
			borderPen = new Pen((fill == TabControlFill.Top) ? color2 : color1);
		}
		
		private Brush GetBrush()
		{
    		int angle = 0;
    		switch (align)
    		{
    			case TabAlignment.Top:
    				angle = 90;
    				break;
    			case TabAlignment.Bottom:
    				angle = 270;
    				break;
    			case TabAlignment.Right:
    				angle = 180;
    				break;
    		}
    		
    		Blend blend;
    		switch (fill)
    		{
    			case TabControlFill.Linear:
    				brush = new LinearGradientBrush(rect, color1, color2, angle, true);
    				break;
    			case TabControlFill.Top:
    				{
    					brush = new LinearGradientBrush(rect, color1, color2, angle, true);
    					blend = new Blend();
    					float[] relativeIntensities = {1.0F, 0.8F, 0.6F, 0.4F, 0.2F, 0.0F, 0.0F};
    					float[] relativePositions = {0.0F, 0.05F, 0.1F, 0.15F, 0.2F, 0.3F, 1.0F};
    					blend.Factors = relativeIntensities;
    					blend.Positions = relativePositions;
    					((LinearGradientBrush)brush).Blend = blend;
    				}
    				break;
    			case TabControlFill.Center:
    				{
    					brush = new LinearGradientBrush(rect, color1, color2, angle, true);
    					blend = new Blend();
    					float[] relativeIntensities = {0.0F, 0.5F, 1.0F, 0.5F, 0.0F};
    					float[] relativePositions = {0.0F, 0.3F, 0.5F, 0.7F, 1.0F};
    					blend.Factors = relativeIntensities;
    					blend.Positions = relativePositions;
    					((LinearGradientBrush)brush).Blend = blend;
    				}
    				break;
    			case TabControlFill.Bottom:
    				{
    					brush = new LinearGradientBrush(rect, color1, color2, angle + 180, true);
    					blend = new Blend();
    					float[] relativeIntensities = {1.0F, 0.8F, 0.6F, 0.4F, 0.2F, 0.0F, 0.0F};
    					float[] relativePositions = {0.0F, 0.05F, 0.1F, 0.15F, 0.2F, 0.3F, 1.0F};
    					blend.Factors = relativeIntensities;
    					blend.Positions = relativePositions;
    					((LinearGradientBrush)brush).Blend = blend;
    				}
    				break;
    			default:
    				brush = new SolidBrush(color1);
    				break;
    		}
    		return brush;
		}
		
		#endregion
	}
		
	/// <summary>
	/// Класс предназначен для редактирования свойств типа cnTabColor
	/// </summary>
	public class cnColorConverter : ExpandableObjectConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if ((destinationType == typeof(InstanceDescriptor)) && (value is cnTabColor))
			{
				ConstructorInfo ci = typeof(cnTabColor).GetConstructor(
					new Type[] {
						typeof(Color), 
						typeof(Color), 
						typeof(TabControlFill)});
				if (ci != null)
				{
					cnTabColor c = ((cnTabColor)value);
					return new InstanceDescriptor(
						ci,
						new object[] {
							c.Color1, 
							c.Color2, 
							c.Fill}
					);
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(InstanceDescriptor))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value is InstanceDescriptor)
			{
				InstanceDescriptor id = (value as InstanceDescriptor);
				IEnumerator ie = id.Arguments.GetEnumerator();
				object[] obj = new object[4];
				int i = 0;
				while (ie.MoveNext())
					obj[i++] = ie.Current;
				return new cnTabColor((Color)obj[0], (Color)obj[1], (TabControlFill)obj[2]);
			}
			return base.ConvertFrom(context, culture, value);
		}		
	}
}
