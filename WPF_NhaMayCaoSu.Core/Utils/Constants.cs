
namespace WPF_NhaMayCaoSu.Core.Utils
{
    public class Constants
    {
        public const string ErrorMessageMissingInfo = "Xin hãy nhập tất cả thông tin";
        public const string TitlePleaseTryAgain = "Vui lòng thử lại";
        public const string SuccessMessageCreateAccount = "Tạo tài khoản thành công";
        public const string TitleRegisterSuccess = "Đăng ký thành công";

        public const string BrokerStartedLog = "Broker đã được khởi động";
        public const string BrokerStoppedLog = "Broker đã dừng";
        public const string BrokerStartFailedLog = "Không thể khởi động broker";
        public const string BrokerStopFailedLog = "Không thể dừng broker";
        public const string BrokerRestartFailedLog = "Không thể khởi động lại broker";

        public const string ErrorMessageBrokerStart = "Có lỗi xảy ra khi khởi động broker";
        public const string ErrorMessageBrokerStop = "Có lỗi xảy ra khi dừng broker";
        public const string ErrorMessageBrokerRestart = "Có lỗi xảy ra khi khởi động lại broker";
        public const string ErrorTitle = "Lỗi";

        public const string StatusOnline = "Trực tuyến";
        public const string StatusOffline = "Ngoại tuyến";
        public const string StatusError = "Lỗi";

        public const string ClientIdLabel = "Mã khách hàng";
        public const string ClientIpLabel = "IP khách hàng";

        public const string ErrorMessageRetrieveCamera = "Không thể lấy thông tin của camera.";
        public const string ErrorMessageLoadCameraUrls = "Lỗi khi tải các URL của camera";
        public const string ErrorMessageConnectCamera1 = "Không thể kết nối tới Camera 1. Vui lòng kiểm tra URL.";
        public const string ErrorMessageConnectCamera2 = "Không thể kết nối tới Camera 2. Vui lòng kiểm tra URL.";
        public const string ErrorMessageSaveCameraUrls = "Lỗi khi lưu các URL của camera";
        public const string ErrorMessageCaptureFrameCamera1 = "Không thể chụp khung hình từ Camera 1.";
        public const string ErrorMessageCaptureFrameCamera2 = "Không thể chụp khung hình từ Camera 2.";

        public const string SuccessMessageConnectCamera1 = "Đã kết nối thành công tới Camera 1.";
        public const string SuccessMessageConnectCamera2 = "Đã kết nối thành công tới Camera 2.";
        public const string SuccessMessageSaveCameraUrls = "Đã lưu các URL của camera thành công.";
        public const string SuccessMessageCaptureFrameCamera1 = "Đã chụp hình từ Camera 1.";
        public const string SuccessMessageCaptureFrameCamera2 = "Đã chụp hình từ Camera 2.";

        public const string ErrorMessageSelectCustomer = "Vui lòng chọn một khách hàng để cập nhật!";
        public const string ErrorTitleSelectCustomer = "Chọn một";

        public const string ErrorMessageMissingFields = "Vui lòng nhập đầy đủ thông tin!";
        public const string ErrorMessageInvalidRFID = "RFID Code phải là số hợp lệ!";
        public const string ErrorMessageInvalidStatus = "Status phải là 0 (inactive) hoặc 1 (active)!";

        public const string ErrorTitleValidation = "Lỗi xác thực";

        public const string SuccessMessageCreateCustomer = "Khách hàng {0} đã được tạo thành công!";
        public const string SuccessMessageUpdateCustomer = "Khách hàng {0} đã được cập nhật thành công!";
        public const string SuccessTitle = "Thành công";

        public const string ModeLabelAddCustomer = "Thêm khách hàng mới";
        public const string ModeLabelEditCustomer = "Chỉnh sửa khách hàng";

        public const string ErrorMessageCameraServiceNotInitialized = "Dịch vụ camera chưa được khởi tạo.";
        public const string ErrorMessageRetrieveNewestCamera = "Không thể lấy thông tin camera mới nhất.";
        public const string ErrorMessageConnectCameras = "Không thể kết nối tới một hoặc cả hai camera. Vui lòng kiểm tra các URL.";
        public const string ErrorMessageNoFrameAvailableCamera1 = "Không có khung hình nào từ Camera 1.";
        public const string ErrorMessageNoFrameAvailableCamera2 = "Không có khung hình nào từ Camera 2.";

        public const string SuccessMessagePhotoSaved = "Ảnh đã được lưu vào {0}.";

        public const string ErrorMessageInvalidLogin = "Tên người dùng hoặc mật khẩu không hợp lệ.";
        public const string ErrorTitleLoginFailed = "Đăng nhập thất bại";

        public const string ErrorMessageSelectSale = "Vui lòng chọn một giao dịch để cập nhật!";
        public const string ErrorTitleSelectSale = "Chọn một";

        public const string SuccessMessageSaleCreated = "Giao dịch đã được tạo thành công!";
        public const string SuccessMessageSaleUpdated = "Giao dịch đã được cập nhật thành công!";
        public const string SuccessMessageCapturedFrame = "Đã chụp khung hình từ Camera {0}.";
        public const string ErrorMessageCaptureFrameFailed = "Không thể chụp khung hình từ Camera {0}.";
        public const string ErrorMessageOpenCameraFailed = "Không thể mở Camera {0}.";
        public const string ErrorMessageInvalidCameraUrl = "URL Camera {0} không hợp lệ.";
        public const string ErrorMessageCaptureImage = "Lỗi khi chụp ảnh từ Camera {0}: {1}";

        public const string ModeLabelAddSale = "Thêm Sale mới";
        public const string ModeLabelEditSale = "Chỉnh sửa Sale";

        public const string OpenServerText = "Mở máy chủ";
        public const string CloseServerText = "Đóng máy chủ";

        public const string ServerOnlineStatus = "Online";
        public const string ServerOfflineStatus = "Offline";

        public const string BrokerStartErrorMessage = "Đã xảy ra lỗi khi khởi động máy chủ.";
        public const string BrokerStopErrorMessage = "Đã xảy ra lỗi khi dừng máy chủ.";
    }
}
