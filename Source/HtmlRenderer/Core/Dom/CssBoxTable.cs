
using System;
using System.Collections.Generic;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Handlers;
using TheArtOfDev.HtmlRenderer.Core.Utils;
using TheArtOfDev.HtmlRenderer.Core.Parse;

namespace TheArtOfDev.HtmlRenderer.Core.Dom
{
    /// <summary>
    /// CSS box for image element.
    /// </summary>
    internal sealed class CssBoxTable : CssBox
    {

        private class LengthLimits
        {
            double _value;
            double _min;
            double _max;
            double _parentLength;

            public LengthLimits(double parent_length)
            {

                _parentLength = parent_length;
                _value = 0;
                _min = 0;
                _max = 0;
            }

            public void ParseCssBoxHeight(CssBox box)
            {

                if (!(_parentLength == 0 && box.Height.Trim().EndsWith("%")))
                    value = CssValueParser.ParseLength(box.Height, _parentLength, box);
                if (!(_parentLength == 0 && box.MaxHeight.Trim().EndsWith("%")))
                    max = CssValueParser.ParseLength(box.MaxHeight, _parentLength, box);
                if (!(_parentLength == 0 && box.MinHeight.Trim().EndsWith("%")))
                    min = CssValueParser.ParseLength(box.MinHeight, _parentLength, box);
            }
            public void ParseCssBoxWidth(CssBox box)
            {

                if (!(_parentLength == 0 && box.Width.Trim().EndsWith("%")))
                    value = CssValueParser.ParseLength(box.Width, _parentLength, box);
                if (!(_parentLength == 0 && box.MaxWidth.Trim().EndsWith("%")))
                    max = CssValueParser.ParseLength(box.MaxWidth, _parentLength, box);
                if (!(_parentLength == 0 && box.MinWidth.Trim().EndsWith("%")))
                    min = CssValueParser.ParseLength(box.MinWidth, _parentLength, box);
            }
            public double value
            {
                get { return _value; }
                set { if (value != 0 && !(_value > 0 && value <= _value)) _value = value; }
            }

            public double min
            {
                get { return _min; }
                set { if (value != 0 && !(_min > 0 && value <= _min)) _min = value; }
            }
            public double max
            {
                get { return _max; }
                set { if (value != 0 && !(_max > 0 && value >= _max)) _max = value; }
            }


        }

        private class Table
        {
            private Dictionary<string, CssBox> _t = new Dictionary<string, CssBox>();
            public readonly int rows;
            public int cols = 0;

            public Dictionary<int, LengthLimits> cols_limits = new Dictionary<int, LengthLimits>();
            public Dictionary<int, LengthLimits> rows_limits = new Dictionary<int, LengthLimits>();

            public CssBox tableBox;

            public Table(CssBox table_box)
            {
                tableBox = table_box;
                this.rows = tableBox.Boxes.Count;

                var trs = tableBox.Boxes;
                for (int row = 0; row < trs.Count; row++)
                {
                    var tr = trs[row];
                    this.rows_limits[row] = new LengthLimits(tableBox.Size.Height);
                    this.rows_limits[row].ParseCssBoxHeight(tr);

                    var tds = tr.Boxes;
                    foreach (var td in tds)
                    {

                        int col = this.GetFirstEmptyColInRow(row);
                        if (!this.cols_limits.ContainsKey(col))
                            this.cols_limits[col] = new LengthLimits(tableBox.Size.Width);

                        int colspan = GetColSpan(td);
                        int rowspan = GetRowSpan(td);
                        if (rowspan == 1)
                            this.rows_limits[row].ParseCssBoxHeight(td);
                        if (colspan == 1)
                            this.cols_limits[col].ParseCssBoxWidth(td);

                        for (int xr = 0; xr < rowspan; xr++)
                            for (int xc = 0; xc < colspan; xc++)
                                this[row + xr, col + xc] = new CssBoxHr(null, null);
                        this[row, col] = td;

                    }

                }
                this.CleanUp();
            }
            public int GetFirstEmptyColInRow(int row)
            {
                int col = 0;
                while (this[row, col] != null)
                    col++;
                return col;
            }

            public CssBox this[int row, int col]
            {
                get
                {
                    if (_t.ContainsKey(String.Format("{0},{1}", row, col)))
                        return _t[String.Format("{0},{1}", row, col)];
                    else
                        return null;
                }
                set
                {
                    if (row >= rows) return;
                    cols = col + 1 > cols ? col + 1 : cols;
                    _t[String.Format("{0},{1}", row, col)] = value;
                }

            }
            public void CleanUp()
            {
                for (int row = 0; row < rows; row++)
                    for (int col = 0; col < cols; col++)
                    {
                        if (!cols_limits.ContainsKey(col))
                            cols_limits[col] = new LengthLimits(0);
                        if (this[row, col] is CssBoxHr)
                            this[row, col] = null;
                    }



            }

        }

        private bool _widthSpecified;
        private bool _heightSpecified;

        private Table _table;


        public CssBoxTable(CssBox parent, HtmlTag tag) : base(parent, tag)
        {

        }

        /// <summary>
        /// Measures the bounds of box and children, recursively.<br/>
        /// Performs layout of the DOM structure creating lines by set bounds restrictions.<br/>
        /// </summary>
        /// <param name="g">Device context to use</param>
        protected override void PerformLayoutImp(RGraphics g)
        {
            if (Display == CssConstants.None) return;


            CalcTableSize(g);
            _table = new Table(this);
            LayoutCells(g, _table);

            //
            //PerformLayoutWithoutTableWH(g, _table);

        }
        private static void LayoutCells(RGraphics g, Table table)
        {
            double top = table.tableBox.Location.Y;
            double left = table.tableBox.Location.X;
            for (int row = 0; row < table.rows; row++)
            {
                left = table.tableBox.Location.X;
                for (int col = 0; col < table.cols; col++)
                {
                    if (table[row, col] != null)
                    {
                        var td = table[row, col];
                        int colspan = GetColSpan(td);
                        int rowspan = GetRowSpan(td);
                        double height = 0;
                        for (int xr = 0; xr < rowspan; xr++)
                            height += table.rows_limits[row + xr].value;

                        double width = 0;
                        for (int xc = 0; xc < colspan; xc++)
                            width += table.cols_limits[col + xc].value;
                        td.PerformLayout(g);
                        td.Location = new RPoint(left, top);
                        td.Size = new RSize(width, height);
                        //That will automatically set the bottom of the cell
                    }
                    left += table.cols_limits[col].value;
                }
                top += table.rows_limits[row].value;
            }
        }


        private void CalcTableSize(RGraphics g)
        {
            if (Display != CssConstants.None)
            {
                RectanglesReset();
                MeasureWordsSize(g);
            }

            double width = ContainingBlock.Size.Width
                                              - ContainingBlock.ActualPaddingLeft - ContainingBlock.ActualPaddingRight
                                              - ContainingBlock.ActualBorderLeftWidth - ContainingBlock.ActualBorderRightWidth;
            double height = ContainingBlock.Size.Height
                                             - ContainingBlock.ActualPaddingTop - ContainingBlock.ActualPaddingBottom
                                             - ContainingBlock.ActualBorderTopWidth - ContainingBlock.ActualBorderBottomWidth;

            bool _widthSpecified = false;
            bool _heightSpecified = false;
            if (Width != CssConstants.Auto && !string.IsNullOrEmpty(Width))
            {
                width = CssValueParser.ParseLength(Width, width, this);
                _widthSpecified = width > 0 ? true : false;
            }
            if (Height != CssConstants.Auto && !string.IsNullOrEmpty(Height))
            {
                height = CssValueParser.ParseLength(Height, height, this);
                _heightSpecified = height > 0 ? true : false;
            }
            Size = new RSize(_widthSpecified ? width : 0, _heightSpecified ? height : 0);
        }

        private static void PerformLayoutWithoutTableWH(RGraphics g, Table table)
        {

            for (int row = 0; row < table.rows; row++)
            {
                for (int col = 0; col < table.cols; col++)
                {

                }
            }
        }



        /// <summary>
        /// Gets the colspan of the specified box
        /// </summary>
        /// <param name="b"></param>
        private static int GetColSpan(CssBox b)
        {
            string att = b.GetAttribute("colspan", "1");
            int colspan;

            if (!int.TryParse(att, out colspan))
            {
                return 1;
            }

            return colspan;
        }

        /// <summary>
        /// Gets the rowspan of the specified box
        /// </summary>
        /// <param name="b"></param>
        private static int GetRowSpan(CssBox b)
        {
            string att = b.GetAttribute("rowspan", "1");
            int rowspan;

            if (!int.TryParse(att, out rowspan))
            {
                return 1;
            }

            return rowspan;
        }

    }
}
