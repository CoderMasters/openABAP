function send()
{
	//document.screen.result.value = document.screen.source.value;
	var  http = null;
	if (window.XMLHttpRequest)
	{// code for IE7+, Firefox, Chrome, Opera, Safari
		http = new XMLHttpRequest();
	} else {// code for IE6, IE5
		http = new ActiveXObject("Microsoft.XMLHTTP");
	}

	var url = "compile";
	http.open("POST", url, true);

	//Send the proper header information along with the request
	http.setRequestHeader("Content-type", "text/plain");
	http.setRequestHeader("Content-length", document.screen.source.value.length);
	http.setRequestHeader("Connection", "close");

	http.onreadystatechange = function() {//Call a function when the state changes.
		if(http.readyState == 4 && http.status == 200) {
			document.screen.result.value = http.responseText;
		}
	}
	http.send(document.screen.source.value);
	
	return false;
}
