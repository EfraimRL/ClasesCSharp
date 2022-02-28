
        /// <summary>
        /// Compara que exista el renglon dentro de la coleccion. pk es un arreglo de las columnas que se deben comparar en caso de no contar con Llave Primaria.
        /// Si no tiene llave primaria compara que sea exactamente el mismo renglon.
        /// </summary>
        private static int CompareIn(DataRowCollection rows, DataRow row, int[] pk = null)
        {
            /* Limpieza de pk */
            pk = pk is null ? pk : pk.GroupBy(x => x)  /*En caso de que el grupo tenga sólo 1 elemento*/ .Where(x => x.Count() == 1) /*Selecciono su Key (el entero)*/  .Select(x => x.Key)/*Convierto a Array.*/  .ToArray();
            string index = rows.Count > 0 && rows[0].Table.PrimaryKey.Length > 0 ? (rows[0].Table.PrimaryKey.First().ColumnName) : (row.Table.PrimaryKey.Length > 0 ? row.Table.PrimaryKey.First().ColumnName : "-1");
            bool existeConLlavePrimaria = index != "-1" && rows.Contains(row[index]);
            foreach (DataRow _row in rows)
            {
                bool hayPk = true;
                bool sonIguales = true;
                for (int i = 0; i < _row.ItemArray.Length; i++)
                {
                    bool fechasIguales = false;
                    string type = row[i].GetType() + "-" + _row[i].GetType();
                    if (_row[i] is DateTime || row[i] is DateTime)
                    {
                        DateTime f1, f2;
                        f1 = _row[i] is DateTime ? ((DateTime)_row[i]) : DateTime.Parse(_row[i].ToString());//
                        f2 = row[i] is DateTime ? ((DateTime)row[i]) : DateTime.Parse(row[i].ToString());
                        fechasIguales = f1 == f2;
                    }
                    bool realesIguales = _row[i] is double || row[i] is double && _row[i].ToDouble() == row[i].ToDouble();
                    if (_row[i].ToString() == row[i].ToString() || fechasIguales || realesIguales) { if (!existeConLlavePrimaria && hayPk && pk != null && pk[pk.Length - 1] == i) { existeConLlavePrimaria = true; } }
                    else { sonIguales = false; if (!existeConLlavePrimaria && ((pk != null && pk.Length > 0 && isIn(pk, i)) || pk == null || pk.Length < 1)) { hayPk = false; } if (!sonIguales && !hayPk) { break; } }
                }
                if (sonIguales) { return 1; }
            }
            return existeConLlavePrimaria ? 0 : -1;
        }

        private static bool isIn(int[] pk, int i)
        {
            return pk.Where(x => x == i).Count() > 0;
        }

        /// <summary>
        /// Compara que exista el renglon dentro de la colección. Si no tiene Llave Primaria, compara que sea exactamente el mismo renglon.
        /// </summary>
        private static bool Contains(DataRowCollection rows, DataRow row) { return CompareIn(rows, row) > -1; }
        /// <summary>
        /// Crea un insert usando el renglon que recibe como parametro. Detecta automaticamente si tiene llave primaria la tabla del renglon.
        /// Recibe un renglon con los valores, el nombre de la tabla, el nombre de las columnas, opcional insert ignore, on duplicate update.
        /// </summary>
        private static string CreateInsert(DataRow row, string table, DataColumnCollection _columns, bool _ignore = false, bool _onDuplicate = false)
        {
            if (_onDuplicate && !(row.Table.PrimaryKey.Length > 0)) { throw new Exception("La tabla no tiene Primary Key, asegurate que la tabla en la BD y en el DataTable contengan la llave primaria"); }
            string ignore = _ignore ? " IGNORE " : "";
            string ret = "INSERT " + ignore + "INTO " + table;
            string values = "(", columns = "(", onDuplicate = "";
            for (int i = 0; i < row.ItemArray.Count(); i++)
            {
                string colName = _columns[i].ColumnName;
                object value = LimpiarValor(row[i]);
                if (i != 0) { columns += ","; values += ","; }
                columns += colName;
                string type = value.GetType().ToString();
                /*string tempDate = value is DateTime ? ((DateTime)value).ToString("HH:mm:ss") : "";
                value = value is DateTime || value is string || value is char || value is TimeSpan ?
                    "'" + (value is DateTime && tempDate == "00:00:00" ? ((DateTime)value).ToString("yyyy-MM-dd") : value) + "'" :
                    (value is DBNull ? "NULL" : value);*/
                values += value;
                if (_onDuplicate)
                {
                    onDuplicate += string.Format("{2}{0}={1}", colName, value, (i != 0 && onDuplicate.Replace(" ", "") != "") ? ", " : " ");
                }
            }
            string primaryKeys = "";
            bool isFirst = true;
            foreach (DataColumn col in row.Table.PrimaryKey)
            {
                primaryKeys += (isFirst ? "" : ",") + col.ColumnName; isFirst = false;
            }
            onDuplicate = _onDuplicate ? (" ON DUPLICATE KEY UPDATE /*" + primaryKeys + "*/" + onDuplicate) : "";
            ret += columns + ") VALUES " + values + ")" + onDuplicate + ";";
            return ret;
        }

        /// <summary>
        /// Crea una consulta Update con el renglon recibido.
        /// Recibe el renglon, el nombre de la tabla, el nombre de las columnas, y las llaves primarias en caso de que la tabla del renglon no tenga llave primaria
        /// </summary>
        private static string CreateUpdate(DataRow row, string table, DataColumnCollection _columns, int[] _pk = null)
        {
            string ret = "UPDATE " + table + " SET ";
            string values = "", where = "";
            string primaryKeys = "";
            bool isFirst = true;
            for (int i = 0; i < row.ItemArray.Count(); i++)
            {
                bool notUpdateThis = false;
                foreach (DataColumn col in row.Table.PrimaryKey)
                {
                    if (_columns.IndexOf(col) == i) { notUpdateThis = true; }
                }
                notUpdateThis = notUpdateThis || (!(_pk is null) && _pk.Contains(i));
                string colName = _columns[i].ColumnName,
                    colName_Formated = row[i] is DateTime ? "date(" + colName + ")" : colName;
                object value = LimpiarValor(row[i]);
                string type = value.GetType().ToString();

                where += string.Format("{2}{0}={1}", colName_Formated, value, (i != 0) ? " AND " : " ");

                values += notUpdateThis ? "" : string.Format("{2}{0}={1}", colName, value, (i != 0 && values.Replace(" ", "") != "") ? ", " : " ");
                if (notUpdateThis) { primaryKeys += (isFirst ? " " : " AND ") + colName_Formated + "=" + value; isFirst = false; }
            }
            where = " WHERE " + (isFirst ? where : primaryKeys) + ";";
            ret += values + where;
            return ret;
        }
        /// <summary>
        /// Crea una consulta de Delete.
        /// Recibe un renglon, el nombre de la tabla, una coleccion de columnas para los nombres, y opcional las columnas que son llaves primarias
        /// </summary>
        private static string CreateDelete(DataRow row, string table, DataColumnCollection _columns, int[] _pk = null)
        {
            string ret = "DELETE FROM " + table;
            string where = "";
            string primaryKeys = "";
            bool isFirst = true;
            foreach (DataColumn col in row.Table.PrimaryKey)
            {
                object value = LimpiarValor(row[_columns.IndexOf(col)]);
                primaryKeys += (isFirst ? "" : " AND ") + col.ColumnName + "=" + value; isFirst = false;
            }
            if (isFirst)
            {
                for (int i = 0; i < row.ItemArray.Count(); i++)
                {
                    string colName = _columns[i].ColumnName;
                    object value = LimpiarValor(row[i]);
                    string type = value.GetType().ToString();
                    if (!(_pk is null) && _pk.Contains(i))
                    { primaryKeys += (isFirst ? "" : " AND ") + colName + "=" + value; isFirst = false; }
                    else
                    { where += string.Format("{2}{0}={1}", colName, value, (i != 0 && where.Replace(" ", "") != "") ? " AND " : " "); }
                }
            }
            where = " WHERE " + (isFirst ? where : primaryKeys) + ";";
            return ret + where;
        }


        public static object LimpiarValor(object value)
        {
            object s = "";
            string tempDate = value is DateTime ? ((DateTime)value).ToString("HH:mm:ss") : "";
            s = value is DateTime || value is string || value is char || value is TimeSpan ?
                "'" + (value is DateTime ?
                    (tempDate == "00:00:00" ?
                        ((DateTime)value).ToString("yyyy-MM-dd") :
                        ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss")) :
                    value) + "'" :
                (value is DBNull ? "NULL" : value);
            return s;
        }
