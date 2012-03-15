using UnityEngine;
using System;
using System.Collections.Generic;

public class HtmlTag
{
  /// <summary>
  /// Name of this tag
  /// </summary>
  public string Name { get; set; }

  /// <summary>
  /// Collection of attribute names and values for this tag
  /// </summary>
  public Dictionary<string, string> Attributes { get; set; }

  /// <summary>
  /// True if this tag contained a trailing forward slash
  /// </summary>
  public bool TrailingSlash { get; set; }
};

