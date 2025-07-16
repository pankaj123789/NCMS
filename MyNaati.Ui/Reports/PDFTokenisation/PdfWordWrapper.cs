//using System;
//using System.Collections.Generic;
//using System.Text.RegularExpressions;
//using PdfSharp.Drawing;

//namespace MyNaati.Ui.Reports.PDFTokenisation
//{
//    /// <summary>
//    /// PdfWordWrapper
//    /// wraps text to a specified line width for PDFsharp.
//    /// Programmed by Huysentruit Wouter
//    /// © Copyright by Fastload-Media.be
//    /// </summary>
//    public class PdfWordWrapper
//    {
//        public enum Alignment { Left, Center, Right, Justify }

//        private enum BlockType { Text, Space, LineBreak }

//        private class Block
//        {
//            public Block(string text, BlockType type, XFont font, XBrush brush, XSize size, double descent)
//            {
//                this.Text = text;
//                this.Type = type;
//                this.Font = font;
//                this.Brush = brush;
//                this.Size = size;
//                this.Descent = descent;
//            }

//            public string Text;
//            public BlockType Type;
//            public XFont Font;
//            public XBrush Brush;
//            public XSize Size;
//            public double Descent;
//            public double X = 0;
//        }

//        private class Line
//        {
//            public List<Block> Blocks = new List<Block>();
//            public XSize Size = new XSize(0, 0);
//            public double Descent = 0;
//        }

//        private XGraphics graphics;
//        private double width;
//        private List<Block> blocks = new List<Block>();
//        private List<Line> lines = new List<Line>();
//        private bool needsProcessing = false;
//        private XSize size = new XSize(0, 0);

//        public PdfWordWrapper(XGraphics graphics, double width)
//        {
//            if (graphics == null)
//                throw new ArgumentNullException("graphics");

//            this.graphics = graphics;
//            this.width = width;
//        }

//        private void CreateBlocks(string text, XFont font, XBrush brush)
//        {
//            double spaceWidth = graphics.MeasureString("X X", font).Width - graphics.MeasureString("XX", font).Width;
//            double lineHeight = font.GetHeight();
//            double descent = -(lineHeight * font.Metrics.Descent / font.Metrics.XHeight);

//            Regex regex = new Regex(@"(\s)|(\S+)");
//            text = text.Replace('\r', '\n');

//            foreach (Match match in regex.Matches(text))
//            {
//                string value = match.Groups[0].Value;

//                if ((value == null) || (value == ""))
//                    continue;

//                if (value == " ")
//                {
//                    XSize size = graphics.MeasureString(value, font);
//                    size.Width = spaceWidth;
//                    size.Height = lineHeight;
//                    blocks.Add(new Block(value, BlockType.Space, font, null, size, descent));
//                    continue;
//                }

//                if (value == "\n")
//                {
//                    blocks.Add(new Block(value, BlockType.LineBreak, null, null, new XSize(0, lineHeight), descent));
//                    continue;
//                }

//                XSize blockSize = graphics.MeasureString(value, font);

//                while (blockSize.Width > width)
//                {
//                    for (int i = value.Length - 1; i > 0; i--)
//                    {
//                        if (i > value.Length)
//                            i = value.Length;
//                        string part = value.Substring(0, i);

//                        blockSize = graphics.MeasureString(part, font);
//                        blockSize.Height = lineHeight;

//                        if (blockSize.Width > width)
//                            continue;

//                        blocks.Add(new Block(part, BlockType.Text, font, brush, blockSize, descent));

//                        value = value.Substring(i, value.Length - i);

//                        if (value.Length == 0)
//                        {
//                            blockSize = XSize.Empty;
//                            break;
//                        }
//                    }
//                }

//                if (value.Length > 0)
//                    blocks.Add(new Block(value, BlockType.Text, font, brush, blockSize, descent));
//            }

//            needsProcessing = true;
//        }

//        public void Process()
//        {
//            Line line = new Line();
//            bool lineBreak = false;

//            size.Width = width;
//            size.Height = 0;

//            foreach (Block block in blocks)
//            {
//                if (((line.Size.Width + block.Size.Width) > width) || lineBreak)
//                {   // Create a new line...
//                    // Skip space at the end of a line
//                    if (line.Blocks.Count > 0)
//                    {
//                        Block lastBlock = line.Blocks[line.Blocks.Count - 1];
//                        if (lastBlock.Type == BlockType.Space)
//                        {
//                            line.Size.Width -= lastBlock.Size.Width;
//                            line.Blocks.Remove(lastBlock);
//                        }
//                    }

//                    // Add line to list
//                    lines.Add(line);

//                    // Create new line
//                    line = new Line();

//                    // Skip space at beginning of a new line
//                    if (block.Type == BlockType.Space)
//                        continue;

//                    lineBreak = false;
//                }

//                block.X = line.Size.Width;
//                line.Size.Width += block.Size.Width;
//                line.Blocks.Add(block);

//                if (block.Descent > line.Descent)
//                    line.Descent = block.Descent;

//                if (block.Size.Height > line.Size.Height)
//                {
//                    size.Height -= line.Size.Height;
//                    size.Height += block.Size.Height;
//                    line.Size.Height = block.Size.Height;
//                }

//                if (block.Type == BlockType.LineBreak)
//                    lineBreak = true;
//            }

//            if (line.Blocks.Count > 0)
//                lines.Add(line);

//            // Override the measured width
//            size.Width = width;

//            foreach (Line l in lines)
//                size.Height += l.Descent;

//            needsProcessing = false;
//        }

//        public void Clear()
//        {
//            blocks.Clear();
//            lines.Clear();
//            needsProcessing = false;
//        }

//        public void Add(string text, XFont font, XBrush brush)
//        {
//            CreateBlocks(text, font, brush);
//        }

//        public void Draw(XGraphics g, double x, double y, Alignment align)
//        {
//            if (needsProcessing)
//                Process();

//            double lineOffsetX = 0;
//            double lineOffsetY = 0;
//            double justifySpacing = 0;
//            double justifyOffsetX = 0;

//            foreach (Line line in lines)
//            {
//                lineOffsetY += line.Size.Height;

//                if (align == Alignment.Center)
//                    lineOffsetX = (size.Width - line.Size.Width) / 2;

//                if (align == Alignment.Right)
//                    lineOffsetX = size.Width - line.Size.Width;

//                if (align == Alignment.Justify)
//                {
//                    int spaceCount = 0;
//                    foreach (Block block in line.Blocks)
//                        if (block.Type == BlockType.Space)
//                            spaceCount++;

//                    justifyOffsetX = 0;
//                    justifySpacing = (size.Width - line.Size.Width) / (double)spaceCount;
//                }

//                foreach (Block block in line.Blocks)
//                {
//                    if (block.Type == BlockType.LineBreak)
//                        continue;

//                    if (block.Type == BlockType.Space)
//                    {
//                        if (align == Alignment.Justify)
//                            justifyOffsetX += justifySpacing;
//                        continue;
//                    }

//                    g.DrawString(block.Text, block.Font, block.Brush, x + block.X + lineOffsetX + justifyOffsetX, y + lineOffsetY);
//                }

//                lineOffsetY += line.Descent;
//            }
//        }

//        public XSize Size
//        {
//            get { return size; }
//        }
//    }
//}