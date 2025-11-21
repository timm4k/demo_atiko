using System;

namespace atiko.Common;

public class ItemEventArgs<T> : EventArgs
{
    public T? Item { get; set; }
}
