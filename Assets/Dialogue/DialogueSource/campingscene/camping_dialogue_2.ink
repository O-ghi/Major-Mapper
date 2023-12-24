INCLUDE global.ink

-> main

=== main ===
Hiện tôi cũng đang cải thiện lại chất lượng đất bằng phân bón.
Do không chăn nuôi gì nên khu này thì chỉ còn cách bón phân đạm cá.
Tôi đang cần thêm 1 lượng nhỏ cá để thực hiện nhưng đang khá bận nên không làm được
cậu có thể giúp tôi không?
-> opts
= opts
* [Đồng ý]
    Thật tốt quá, hiện tôi chỉ cần thêm 3 con cá, cậu có thể dùng chỗ cần câu bên hồ để câu cá. Cảm ơn cậu.
    ~acceptTask()
    -> DONE
* [Từ chối]
    Việc này cũng thú vị mà, đổi lại tôi có thể cho cậu chỗ hoa quả tôi thu hoạch được nếu thành công.
    -> opts1
= opts1
* [Đồng ý]
    Thật tốt quá, hiện tôi chỉ cần thêm 3 con cá, cậu có thể dùng chỗ cần câu bên hồ để câu cá. Cảm ơn cậu. 
    ~acceptTask()
    -> DONE
* [Từ chối]
    Thật sự cậu không có thời gian sao, thôi được rồi, dù khá tiếc nhưng không sao cả.
    -> DONE
=== openagain ===
Ồ bạn nghĩ lại rồi à?
* [Đồng ý]
    Thật tốt quá, hiện tôi chỉ cần thêm 3 con cá, cậu có thể dùng chỗ cần câu bên hồ để câu cá. Cảm ơn cậu.
    ~acceptTask()
    -> DONE
* [Từ chối]
    Việc này cũng thú vị mà, đổi lại tôi có thể cho cậu chỗ hoa quả tôi thu hoạch được nếu thành công.
=== reward ===
Thật sự rất cảm ơn cậu, giờ tôi đã có đủ nguyên liệu để thực hiện rồi.
~ submitTask()
