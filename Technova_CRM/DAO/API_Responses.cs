namespace Technova_CRM.DAO
{
    public class API_Responses
    {
        public class Code
        {
            public static int Success = 200;
            public static int Invalid_Token = 310;
            public static int Invalid_Param = 400;
            public static int Invalid_User = 300;
            public static int Invalid_ID = 350;
            public static int Invalid_Data = 450;
            public static int Exception = 500;
        }

        public class Language
        {
            public class VN
            {
                public static string alertRequiredParam = "Parameters bắt buộc";
                public static string alertIdDoesNotExist = "ID không tồn tại";
                public static string alertCreateDataSuccess = "Tạo mới dữ liệu thành công";
                public static string alertUpdateDataSuccess = "Cập nhật dữ liệu thành công";
                public static string alertDeleteDataSuccess = "Xóa dữ liệu thành công";
                public static string alertDeleteDataFailed = "Tất cả dữ liệu liên quan phải được xóa trước khi xóa dữ liệu này";
                public static string alertNotFoundData = "Không tìm thấy dữ liệu";
                public static string alertDataUsing = "Dữ liệu đang được sử dụng";
                public static string alertInvalidUsPw = "Tài khoản hoặc mật khẩu không đúng";
                public static string alertUsAlreadyExisted = "Tài khoản đã tồn tại";
                public static string alertUsAlreadyExistedData = "Dữ liệu đã tồn tại";
                public static string alertException = "Đã có lỗi xảy ra.";
                public static string alertRequiredUsPw = "Tài khoản hoặc mật khẩu bắt buộc";
                public static string alertInvalidToken = "Token không hợp lệ hoặc đã hết hạn";
                public static string alertInvalidByStatus = "Trạng thái hiện tại không thể thao tác";
                public static string alertInvalidContractLines = "Vui lòng thêm chi tiết đơn hàng vào hợp đồng này";
            }

            public class EN
            {
                public static string alertRequiredUsPw = "Username & Password are required";
                public static string alertInvalidUsPw = "Username & Password is invalid";
                public static string alertRequiredParam = "Parameters are required";
                public static string alertIdDoesNotExist = "ID does not exist";
                public static string alertInvalidToken = "Token is invalid or has expired";
                public static string alertUpdateDataSuccess = "Update data successful";
                public static string alertCreateDataSuccess = "Create data successful";
                public static string alertDeleteDataSuccess = "Delete data successful";
                public static string alertDeleteDataFailed = "All sub-data must be deleted first before deleting this data";
                public static string alertNotFoundData = "Cannot found data";
                public static string alertDataUsing = "Data is being used";
                public static string alertException = "An error has occurred";
                public static string alertInvalidByStatus = "Currently the status is inoperable";
                public static string alertUsAlreadyExisted = "The account already exists";
                public static string alertUsAlreadyExistedData = "Data already exists";
                public static string alertInvalidContractLines = "Please add line items to this contract";
            }
        }
    }
}
