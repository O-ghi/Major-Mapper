INCLUDE global.ink

-> main

=== main ===
Xin chào, cậu mới đến đây đúng không? 
tôi chưa từng gặp cậu ở trong làng bao giờ.
*[Dạ đúng rồi]
Lâu rồi mới có khách đấy, cậu đi theo đoàn đến đây à?
    * *[Không, tôi thích một mình khám phá hơn]
    -> nextq   //tính điểm ở đây
    * *[Đúng rồi, tôi thích đồng hành với mọi người hơn]
    -> nextq
= nextq
    À ra là vậy, mà tiếc là giờ làng tôi cũng không có gì thú vị cho cậu.
    Lúc đi đến đây cậu có để ý gì không?
* [Cây cối không được tốt cho lắm] 
Đúng vậy, do cứ trồng liên tục mà không bù lại phần dinh dưỡng mất đi của đất, nên hiện tại cây rất khó phát triển trên mảnh đất này.
 ~submitTask()
* [Không có gì lạ cả]
Hmmm, rõ ràng vậy mà cậu không nhìn ra ư!
Thôi được rồi, xung quanh đây cây cối hầu như héo úa hết rồi đấy.
 ~submitTask() 

->DONE