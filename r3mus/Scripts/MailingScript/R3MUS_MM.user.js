// ==UserScript==
// @name         	R3MUS Mailing List Copy Enabler
// @namespace     	http://jaynestown.rocks
// @description	Enables the mailing list box for copying
// @include	https://gate.eveonline.com/Mail/ComposeForward/*
// @include	https://gate.eveonline.com/Mail/MailingList/145182300
// @version       	0.0.1.1
// @grant 	none
// ==/UserScript==

if(document.URL.indexOf('ComposeForward/350710023') > -1)
{
	EditRecruitmentMail();
}
else if(document.URL.indexOf('MailingList/145182300') > -1)
{
	AddLinkToRecruitmentMail();
}

function AddLinkToRecruitmentMail()
{
	var newlink, newspan;
	try
	{
		newlink = document.createElement('a');
		newlink.setAttribute('href', 'https://gate.eveonline.com/Mail/ComposeForward/350710023');
		newlink.setAttribute('target', 'blank');				
		newspan = document.createElement('span');
		newspan.innerHTML = 'Recruitment Mail';
		
		newlink.appendChild(newspan);
		document.getElementsByClassName('buttonsContainer')[0].appendChild(newlink);
	}
	catch(ex)
	{
		//alert(ex.message);
	}
}

function EditRecruitmentMail()
{
	var text;
	var index;
	var searchText = "To: <b></b><br /><br />";
	
	try
	{
		text = document.getElementById("subject").value.replace("Fw: ", "");
		document.getElementById("subject").value = text;
		
		index = document.getElementById("mailContents").value.indexOf(searchText) + searchText.length;
		text = document.getElementById("mailContents").value.substring(index, document.getElementById("mailContents").value.length);
		document.getElementById("mailContents").value = text;
	}
	catch(ex)
	{
		//alert(ex.message);
	}
}