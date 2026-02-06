using Clone_KySoDienTu.Models.Dtos.QuanLyChamCong;
using System.Collections.Generic;

namespace Clone_KySoDienTu.Service.QuanLyChamCong
{
    public interface IExportFile
    {
        byte[] GenerateSummariesTableDocx(
            BangTongHopDownloadRequest req,
            List<BangTongHopDownloadResponse> phieuDanhGia);

        byte[] GenerateSummariesTableExcel(
            BangTongHopDownloadRequest req,
            List<BangTongHopDownloadResponse> phieuDanhGia);
    }
}