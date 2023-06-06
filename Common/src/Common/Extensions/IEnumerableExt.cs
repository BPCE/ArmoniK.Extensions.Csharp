// This file is part of the ArmoniK project
//
// Copyright (C) ANEO, 2021-$CURRENT_YEAR$. All rights reserved.
//   W. Kirschenmann   <wkirschenmann@aneo.fr>
//   J. Gurhem         <jgurhem@aneo.fr>
//   D. Dubuc          <ddubuc@aneo.fr>
//   L. Ziane Khodja   <lzianekhodja@aneo.fr>
//   F. Lemaitre       <flemaitre@aneo.fr>
//   S. Djebbar        <sdjebbar@aneo.fr>
//   J. Fonseca        <jfonseca@aneo.fr>
//   D. Brasseur       <dbrasseur@aneo.fr>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY, without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;

namespace ArmoniK.DevelopmentKit.Common.Extensions;

/// <summary>
///   Convert IEnumerable byte to IEnumerable double
/// </summary>
public static class IEnumerableExt
{
  /// <summary>
  ///   Convert IEnumerable byte to IEnumerable double
  /// </summary>
  /// <param name="arr"></param>
  /// <returns></returns>
  public static IEnumerable<double> ConvertToArray(this IEnumerable<byte> arr)
  {
    var bytes = arr as byte[] ?? arr.ToArray();

    var values = new double[bytes.Count() / sizeof(double)];

    for (var i = 0; i < values.Length; i++)
    {
      values[i] = BitConverter.ToDouble(bytes,
                                        i * 8);
    }

    return values;
  }

  /// <summary>
  ///   Returns an enumerable with an index
  /// </summary>
  /// <typeparam name="T">Value type</typeparam>
  /// <param name="source">Source enumerable</param>
  /// <returns>Enumerable of tuples composed of the item and its index</returns>
  public static IEnumerable<(T Item, int Index)> WithIndex<T>(this IEnumerable<T> source)
    => source.Select((item,
                      index) => (item, index));
}
