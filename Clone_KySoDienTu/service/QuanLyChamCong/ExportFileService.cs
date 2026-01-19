using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Clone_KySoDienTu.Models.Dtos.QuanLyChamCong;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Clone_KySoDienTu.Service.QuanLyChamCong
{
    public class ExportFileService : IExportFile
    {
        public byte[] GenerateSummariesTableDocx(
            BangTongHopDownloadRequest req,
            List<BangTongHopDownloadResponse> phieuDanhGia)
        {
            using (var ms = new MemoryStream())
            {
                using (var doc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document))
                {
                    var main = doc.AddMainDocumentPart();
                    main.Document = new Document(new Body());
                    var body = main.Document.Body;

                    // ====== Tiêu đề & thông tin dòng đầu ======
                    body.Append(ParaCenterBold("BẢNG TỔNG HỢP"));
                    body.Append(ParaCenterBold("KẾT QUẢ THỰC HIỆN NHIỆM VỤ CỦA NGƯỜI LAO ĐỘNG"));

                    var thangText = (req.FromMonth == req.ToMonth && req.FromYear == req.ToYear)
                                    ? $"{req.FromMonth}/{req.FromYear}"
                                    : $"{req.FromMonth}/{req.FromYear} - {req.ToMonth}/{req.ToYear}";

                    // Đơn vị & Tháng đánh giá
                    body.Append(
                        ParaLeftRightBold(
                            "Đơn vị:", req.PhongBan ?? "", 
                            "Tháng đánh giá:", thangText)
                        );

                    // ====== Bảng dữ liệu ======
                    body.Append(CreateScoredTable(phieuDanhGia));

                    body.Append(SmallSpacingParagraph());

                    // ====== Chữ ký cuối trang ======
                    body.Append(
                        ParaRight($"Tp.HCM, ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}")
                        );
                    
                    body.Append(
                        ParaLeftTabRight($"TRƯỞNG {req.PhongBan.ToUpper() ?? ""}", "THỰC HIỆN LẬP BIỂU")
                        );

                    body.Append(EmptyLine());
                    body.Append(EmptyLine());

                    body.Append(
                        ParaLeftTabRight($"{req.NguoiTruongDonVi.ToUpper() ?? ""}", $"{req.NguoiLapBieu.ToUpper() ?? ""}")
                        );

                    // ===== Set khổ giấy xoay ngang (Landscape) =====
                    body.Append(SetToLandscape());

                    main.Document.Save();
                }
                return ms.ToArray();
            }
        }

        public byte[] GenerateSummariesTableExcel(
            BangTongHopDownloadRequest req,
            List<BangTongHopDownloadResponse> items)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("BangTongHop");

                // ===== Title =====
                ws.Cells["A1"].Value = "BẢNG TỔNG HỢP";
                ws.Cells["A2"].Value = "KẾT QUẢ THỰC HIỆN NHIỆM VỤ CỦA NGƯỜI LAO ĐỘNG";
                ws.Cells["A1:A2"].Style.Font.Bold = true;
                ws.Cells["A1:A2"].Style.Font.Size = 16;
                ws.Cells["A1:A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A1:G1"].Merge = true;
                ws.Cells["A2:G2"].Merge = true;

                // ===== Sub Info =====
                var thangText = (req.FromMonth == req.ToMonth && req.FromYear == req.ToYear)
                    ? $"{req.FromMonth}/{req.FromYear}"
                    : $"{req.FromMonth}/{req.FromYear} - {req.ToMonth}/{req.ToYear}";

                ws.Cells["A4"].Value = $"Đơn vị: {req.PhongBan}";
                ws.Cells["A5"].Value = $"Tháng đánh giá: {thangText}";

                // ===== Table Header =====
                int row = 7;
                ws.Cells[row, 1].Value = "STT";
                ws.Cells[row, 2].Value = "Mã NV";
                ws.Cells[row, 3].Value = "Họ tên";
                ws.Cells[row, 4].Value = "Chức danh";
                ws.Cells[row, 5].Value = "Tổng điểm";
                ws.Cells[row, 6].Value = "Xếp loại";
                ws.Cells[row, 7].Value = "Ghi chú";

                using (var rng = ws.Cells[row, 1, row, 7])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(255, 211, 211, 211);
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    rng.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                // ===== Data Rows =====
                int idx = 0;
                foreach (var item in items)
                {
                    row++;
                    idx++;

                    ws.Cells[row, 1].Value = idx;
                    ws.Cells[row, 2].Value = item.MaNhanVien;
                    ws.Cells[row, 3].Value = item.HoVaTen;
                    ws.Cells[row, 4].Value = item.ChucVu;
                    ws.Cells[row, 5].Value = item.DiemText;
                    ws.Cells[row, 6].Value = item.XepLoai;
                    ws.Cells[row, 7].Value = item.GhiChu;

                    // Căn chỉnh theo cột
                    ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[row, 7].Style.WrapText = true;

                    // In đậm + nền xám cho dòng đặc biệt
                    if (item.IsBold == 1)
                    {
                        ws.Cells[row, 1, row, 7].Style.Font.Bold = true;
                        ws.Cells[row, 1, row, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[row, 1, row, 7].Style.Fill.BackgroundColor.SetColor(255, 230, 230, 230);
                    }

                    // Border từng ô
                    ws.Cells[row, 1, row, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[row, 1, row, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[row, 1, row, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[row, 1, row, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                // Tự động co độ rộng cột
                ws.Cells[1, 1, row, 7].AutoFitColumns();

                return package.GetAsByteArray();
            }
        }

        // ========== Helpers ==========
        private Paragraph ParaCenterBold(string t) => new Paragraph(
            new ParagraphProperties(new Justification { Val = JustificationValues.Center }),
            new Run(new RunProperties(new Bold(), Times(), Font(24)), new Text(t)));

        private Paragraph ParaCenter(string t) => new Paragraph(
            new ParagraphProperties(new Justification { Val = JustificationValues.Center }),
            new Run(new RunProperties(Times(), Font(24)), new Text(t)));

        private Paragraph ParaRightBold(string t) => new Paragraph(
            new ParagraphProperties(new Justification { Val = JustificationValues.Right }),
            new Run(new RunProperties(new Bold(), Times(), Font(16)), new Text(t)));

        private Paragraph ParaRight(string t) => new Paragraph(
            new ParagraphProperties(new Justification { Val = JustificationValues.Right }),
            new Run(new RunProperties(Times(), Font(16)), new Text(t)));

        private Paragraph ParaLeftBold(string t) => new Paragraph(
            new ParagraphProperties(new Justification { Val = JustificationValues.Left }),
            new Run(new RunProperties(new Bold(), Times(), Font(16)), new Text(t)));

        private Paragraph ParaLeft(string t) => new Paragraph(
            new ParagraphProperties(new Justification { Val = JustificationValues.Left }),
            new Run(new RunProperties(Times(), Font(16)), new Text(t)));

        private Paragraph ParaLeftRightBold(string leftLabel, string leftValue, string rightLabel, string rightValue)
        {
            return new Paragraph(
                new ParagraphProperties(
                    new Justification { Val = JustificationValues.Left },
                    new Tabs(
                        new TabStop
                        {
                            Val = TabStopValues.Right,
                            Position = 13960 // căn phải sát mép hiển thị
                        }
                    )
                ),
                // Left: label (bold)
                new Run(
                    new RunProperties(new Bold(), Times(), Font(16)),
                    new Text(leftLabel + " ") { Space = SpaceProcessingModeValues.Preserve }
                ),
                // Left: value (normal)
                new Run(
                    new RunProperties(Times(), Font(16)),
                    new Text(leftValue + " ") { Space = SpaceProcessingModeValues.Preserve }
                ),
                // Tab separator
                new Run(
                    new RunProperties(Times(), Font(16)),
                    new Text("\t") { Space = SpaceProcessingModeValues.Preserve }
                ),
                // Right: label (bold)
                new Run(
                    new RunProperties(new Bold(), Times(), Font(16)),
                    new Text(rightLabel + " ") { Space = SpaceProcessingModeValues.Preserve }
                ),
                // Right: value (normal)
                new Run(
                    new RunProperties(Times(), Font(16)),
                    new Text(rightValue) { Space = SpaceProcessingModeValues.Preserve }
                )
            );
        }

        private Paragraph ParaLeftTabRight(string leftText, string rightText)
        {
            return new Paragraph(
                new ParagraphProperties(
                    new Justification { Val = JustificationValues.Left }, // đảm bảo văn bản align từ trái
                    new Tabs(
                        new TabStop
                        {
                            Val = TabStopValues.Right,
                            Position = 13960 // Giới hạn trong vùng hiển thị thực tế của trang (A4 ngang trừ lề)
                        }
                    )
                ),
                new Run(
                    new RunProperties(Times(), Font(16)),
                    new Text($"{leftText}\t{rightText}") { Space = SpaceProcessingModeValues.Preserve }
                )
            );
        }

        private Table CreateScoredTable(List<BangTongHopDownloadResponse> items)
        {
            var table = new Table();
            table.AppendChild(new TableProperties(
                new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct },
                new TableBorders(
                    new TopBorder { Val = BorderValues.Single, Size = 5 },
                    new BottomBorder { Val = BorderValues.Single, Size = 5 },
                    new LeftBorder { Val = BorderValues.Single, Size = 5 },
                    new RightBorder { Val = BorderValues.Single, Size = 5 },
                    new InsideHorizontalBorder { Val = BorderValues.Single, Size = 5 },
                    new InsideVerticalBorder { Val = BorderValues.Single, Size = 5 }
                )
            ));

            int sttPer = 5;
            int maNvPer = 6;
            int hoTenPer = 31;
            int chucDanhPer = 30;
            int tongDiemPer = 5;
            int xepLoaiPer = 5;
            int ghiChuPer = 18;

            var header = new TableRow();
            header.Append(CreateCell("STT", isHeader: true, widthPercent: sttPer));
            header.Append(CreateCell("Mã NV", isHeader: true, widthPercent: maNvPer));
            header.Append(CreateCell("Họ tên", isHeader: true, widthPercent: hoTenPer));
            header.Append(CreateCell("Chức danh", isHeader: true, widthPercent: chucDanhPer));
            header.Append(CreateCell("Tổng điểm", isHeader: true, widthPercent: tongDiemPer));
            header.Append(CreateCell("Xếp loại", isHeader: true, widthPercent: xepLoaiPer));
            header.Append(CreateCell("Ghi chú", isHeader: true, widthPercent: ghiChuPer));
            table.Append(header);

            if (items.Count > 0)
            {
                foreach (var item in items.Select((value, index) => new { value, index }))
                {
                    var isBold = item.value.IsBold == 1;
                    var row = new TableRow();
                    row.Append(CreateCell((item.index + 1).ToString(), isBold: isBold, widthPercent: sttPer));
                    row.Append(CreateCell(item.value.MaNhanVien, isBold: isBold, widthPercent: maNvPer));
                    row.Append(CreateCell(item.value.HoVaTen, isBold: isBold, widthPercent: hoTenPer));
                    row.Append(CreateCell(item.value.ChucVu, isBold: isBold, widthPercent: chucDanhPer));
                    row.Append(CreateCell(item.value.DiemText, isBold: isBold, widthPercent: tongDiemPer));
                    row.Append(CreateCell(item.value.XepLoai, isBold: isBold, widthPercent: xepLoaiPer));
                    row.Append(CreateCell(item.value.GhiChu, isBold: isBold, widthPercent: ghiChuPer));
                    table.Append(row);
                }
            }

            else
            {
                for (int i = 1; i <= 5; i++)
                {
                    var row = new TableRow();
                    row.Append(CreateCell(i.ToString()));
                    row.Append(CreateCell($"Mã NV {i}"));
                    row.Append(CreateCell($"Họ tên {i}"));
                    row.Append(CreateCell($"Chức danh {i}"));
                    row.Append(CreateCell($"Tổng điểm {i}"));
                    row.Append(CreateCell($"Xếp loại {i}"));
                    row.Append(CreateCell($"Ghi chú {i}"));
                    table.Append(row);
                }
            }

            return table;
        }

        private Paragraph EmptyLine() => new Paragraph(new Run(new Break()));

        private TableCell CreateCell(string text, bool isHeader = false, bool isBold = false, int? widthPercent = null)
        {
            return CreateCell(text, isHeader, JustificationValues.Center, isBold, widthPercent);
        }

        private TableCell CreateCell(string text, bool isHeader, JustificationValues alignment, bool isBold, int? widthPercent = null)
        {
            var runProps = new RunProperties(Times(), Font());

            if (isHeader || isBold)
                runProps.Append(new Bold());

            var cellProps = new TableCellProperties();

            if (widthPercent.HasValue)
            {
                cellProps.Append(new TableCellWidth
                {
                    Width = (widthPercent.Value * 50).ToString(),
                    Type = TableWidthUnitValues.Pct
                });
            }
            else
            {
                cellProps.Append(new TableCellWidth { Type = TableWidthUnitValues.Auto });
            }

            if (isBold && !isHeader)
            {
                cellProps.Append(new Shading
                {
                    Val = ShadingPatternValues.Clear,
                    Color = "auto",
                    Fill = "D9D9D9" // Mã màu xám nhẹ
                });
            }

            var paragraph = new Paragraph(
                new ParagraphProperties(new Justification { Val = alignment }),
                new Run(runProps, new Text(text) { Space = SpaceProcessingModeValues.Preserve })
            );

            return new TableCell(cellProps, paragraph);
        }

        private SectionProperties SetToLandscape()
        {
            var result = 
                new SectionProperties(
                    new PageSize
                    {
                        Width = 16840,   // A4 ngang
                        Height = 11900,
                        Orient = PageOrientationValues.Landscape
                    },
                    new PageMargin
                    {
                        Top = 1440,      // 1 inch
                        Bottom = 1440,
                        Left = 1440,
                        Right = 1440,
                        Header = 720,
                        Footer = 720,
                        Gutter = 0
                    });
            return result;
        }

        private RunFonts Times() => new RunFonts
        {
            Ascii = "Times New Roman",
            HighAnsi = "Times New Roman",
            EastAsia = "Times New Roman",
            ComplexScript = "Times New Roman"
        };

        private DocumentFormat.OpenXml.Wordprocessing.FontSize Font(int pt = 16) => new DocumentFormat.OpenXml.Wordprocessing.FontSize { Val = (pt * 2).ToString() };

        private Paragraph SmallSpacingParagraph()
        {
            return new Paragraph(
                new ParagraphProperties(
                    new SpacingBetweenLines
                    {
                        After = "0", // <= spacing sau paragraph
                        Line = "240", // chiều cao dòng 12pt (nếu muốn kiểm soát)
                        LineRule = LineSpacingRuleValues.Auto
                    }
                )
            );
        }
    }
}