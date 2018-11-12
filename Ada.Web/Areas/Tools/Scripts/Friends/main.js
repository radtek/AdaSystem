var draggerTop=0;
var draggerTopHeight=0;
var height=0;
var main='';
if(Ver==1){
        height=206;
        draggerTopHeight=1200;
    }else if(Ver==2){
        height=336;
        draggerTopHeight=2074;
    }else if(Ver==3){
        height=236;
        draggerTopHeight=2610;
    }else if(Ver==6){
        height=336;
        draggerTopHeight=1658;
    }else if(Ver==7){
        height=206;
        draggerTopHeight=936;
    }
    if(Ver==6 || Ver==7){
                Noti=[0,
            [
                'noti_360'
                ,'noti_alipay'
                ,'noti_baidu'
                ,'noti_changba'
                ,'noti_momo'
                ,'noti_qq'
                ,'noti_qqmusic'
                ,'noti_qzone'
                ,'noti_sina'
                ,'noti_tieba'
                ,'noti_wechat'
                ,'noti_58'
                ,'noti_baidu_video'
                ,'noti_baidusafe'
                ,'noti_ctrip'
                ,'noti_dianping'
                ,'noti_jingdong'
                ,'noti_laiwang'
                ,'noti_netease_news'
                ,'noti_qiyi'
                ,'noti_qq_mail'
                ,'noti_qqpimsecure'
                ,'noti_sohu_video'
                ,'noti_tencent_news'
                ,'noti_tencent_wblog'
                ,'noti_tmall'
                ,'noti_tudou_video'
                ,'noti_xiami'
                ,'noti_xunlei_download'
                ,'noti_yy_yuyin'
            ]
        ];
    }
function getRandom(n,m){
        var c = m-n+1;
        var b= Math.floor(Math.random() * c + n);
        return b;
    }

function saveImg(src) {
    var alink = document.createElement("a");
    alink.href = src;
    alink.download = "IMG.jpg";
    alink.click();
}
$(document).ready(function(){
                 if(Type==0){
                     main='#main';
                     if($('.cContent p').height()>height){
                        $('.cContent p').css({'max-height':height+'px','overflow':'hidden'});
                        $('.cContent p').after('<div class="cFullText">全文</div>');
                    };
                }else{
                    main='#page';
                };

                if(Ver>5){
                    if(Ver==6){
                         Noti[0]=getRandom(0,2);
                     }else if(Ver==7){
                         Noti[0]=getRandom(0,1);
                         main='#page';
                     }

                    if(Noti[0]==2){
                        $('.networkNoti').html(getRandom(1,200)+'K/s');
                        } else if(Noti[0]==1){
                            var IconHteml='';
                            for(var i= 1;i<=getRandom(1,4);i++){
                                IconHteml+='<img src="/Public/Wechat/img/Vivo/'+Noti[1][getRandom(0,10)]+'.png">';
                            }
                            $('.networkNoti').html(IconHteml);
                        }
                }

                $(".main").mCustomScrollbar({
                    theme:"dark-3"
                    ,scrollbarPosition:'outside'
                    ,autoHideScrollbar:true
                    ,callbacks:{
                        onScroll:function(){
                            draggerTop=-this.mcs.top;
                        },
                        whileScrolling:function(){
                            if(!$('#main').hasClass('details')){
                                if(-this.mcs.top>600){
                                    $('#main .top').addClass('light');
                                }else {
                                    $('#main .top').removeClass('light');
                                }
                            }
                        }
                    }
                });

                $("#btn_screen").on("click", function () {
                    html2canvas(document.querySelector(main)).then(canvas => {
                        var dataUrl = canvas.toDataURL('image/jpeg', 0.99);
                        console.log(dataUrl.length);
                        saveImg(dataUrl);
                        $('#main').mCustomScrollbar('scrollTo',draggerTop+draggerTopHeight);
                    });
                    /*html2canvas($(main), {
                        allowTaint: true,
                        taintTest: false,
                        onrendered: function(canvas) {
                            canvas.id = "mycanvas";
                            var dataUrl = canvas.toDataURL();
                            var newImg = document.createElement("img");
                            newImg.src =  dataUrl;
                            saveImg(dataUrl);
                            //$('#main').mCustomScrollbar('scrollTo',draggerTop+draggerTopHeight);
                            //console.log(dataUrl);
                            //window.open(dataUrl)
                        }
                    });*/
                });

            });
