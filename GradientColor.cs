//-----------------------------------------------------------------------
// <copyright file="GradientColor.cs" company="Sergey Teplyashin">
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
// <summary>Defines the GradientColor class.</summary>
//-----------------------------------------------------------------------

namespace ComponentsNET
{
    #region Using directives
    
    using System;
    using System.ComponentModel;
    using System.Drawing;
    
    #endregion
    
    /// <summary>
    /// Класс предназначен для представления заливки элементов
    /// интерфейса и состоит из 2х цветов, а также способа заливки
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class GradientColor
    {
        private Color color1;
        private Color color2;
        private GradientFill fill;

        public GradientColor() : this(Color.White, Color.White, GradientFill.Solid)
        {
        }
        
        public GradientColor(Color color1, Color color2, GradientFill gradientFill)
        {
            this.color1 = color1;
            this.color2 = color2;
            this.fill = gradientFill;
        }
        
        [DefaultValue(typeof(Color), "White")]
        [NotifyParentProperty(true)]
        public Color Color1
        {
            get
            {
                return this.color1;
            }
            
            set
            {
                if (this.color1 != value)
                {
                    this.color1 = value;
                }
            }
        }
        
        [DefaultValue(typeof(Color), "White")]
        [NotifyParentProperty(true)]
        public Color Color2
        {
            get
            {
                return this.color2;
            }
            
            set
            {
                if (this.color2 != value)
                {
                    this.color2 = value;
                }
            }
        }
        
        [DefaultValue(GradientFill.Solid)]
        [NotifyParentProperty(true)]
        public GradientFill Fill
        {
            get
            {
                return this.fill;
            }
            
            set
            {
                if (this.fill != value)
                {
                    this.fill = value;
                }
            }
        }
    }
}
