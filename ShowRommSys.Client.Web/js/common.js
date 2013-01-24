$(function(){
	/* login */
	$("#btnLogin").click(function(){
		if($.trim($("input[name=username]").val())==''){
			$("input[name=username]").select();
		}else if($("input[name=password]").val()==''){
			$("input[name=password]").select();
		}else if($.trim($("input[name=auth]").val())==''){
			$("input[name=auth]").select();
		}else{
			//$("#frmLogin").submit();
			location.href='main.html?t='+(new Date()).valueOf();
		}
		return false;
	});
	/* menu */
	$(".menu .group p").click(function(){$(this).toggleClass('current').nextUntil().toggle();});
	$(".menu .group li a").click(function(){$(".menu .group li a").removeClass('current');$(this).addClass('current');});
	/* buttons */
	$("a.btnDelete").click(function(){return confirm('确定要删除此条数据吗？');});
	$("a.btnCityAll").click(function(){$(this).parent().find("input").attr('checked',true);});
	$("a.btnCityNone").click(function(){$(this).parent().find("input").attr('checked',false);});
	/* multi */
	$("input[name=selall]").click(function(){$("input[name=multi]").attr('checked',$(this).attr('checked')=='checked');});
	$("input[name=multi]").click(function(){$("input[name=selall]").attr('checked',$("input[name=multi]:checked").length==$("input[name=multi]").length);});
	$("#multiDel").click(function(){return $("input[name=multi]:checked").length>0?confirm('确定要删除选中数据吗？'):false;});
	$("#multiVerify").click(function(){return $("input[name=multi]:checked").length>0?confirm('确定要审核通过选中数据吗？'):false;});
	/* filter */
	$(".filter select[name=type]").change(function(){$(".filter select[name=month]").parent().toggle($(this).val()=='month');});
	/* other */
	$(".flex .toggle").click(function(){$(this).text($(this).parents(".flex").children("ul").toggle().css('display')=='none'?'[显示]':'[隐藏]');return false;});

});
