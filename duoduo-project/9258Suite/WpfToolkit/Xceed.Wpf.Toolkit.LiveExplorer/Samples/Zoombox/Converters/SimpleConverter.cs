﻿/**************************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up the Plus Edition at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like http://facebook.com/datagrids

  *************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Xceed.Wpf.Toolkit.LiveExplorer.Samples.Zoombox.Converters
{
  public abstract class SimpleConverter : IValueConverter
  {
    protected abstract object Convert( object value );

    public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
    {
      return this.Convert( value );
    }

    public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
    {
      throw new NotImplementedException();
    }
  }
}
