using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib
{
    public enum ExportCatalogFormat
    {
        Maps,   // Catalog is a list of key-value pairs, where the key is the ID. They are also ordered. More human-readable and perhaps easier to handle in some JSON libraries.
        Lists   // Catalog is an ordered, standard list. The ID is in each list item. More effecient export size and also likely more efficient for typical JSON libraries.
    }
}
