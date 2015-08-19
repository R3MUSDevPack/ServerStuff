// ==UserScript==
// @name         	R3MUS Mailing List Copy Enabler
// @namespace     	http://jaynestown.rocks
// @description	Enables the mailing list box for copying
// @include	https://gate.eveonline.com/Mail/ComposeForward/*
// @include	https://gate.eveonline.com/Mail/MailingList/145182300
// @version       	0.0.1.3
// @grant 	none
// ==/UserScript==

if (document.URL.indexOf('ComposeForward/352241208') > -1) {
    EditRecruitmentMail();
}
else if (document.URL.indexOf('MailingList/145182300') > -1) {
    AddLinkToRecruitmentMail();
}

function AddLinkToRecruitmentMail() {
    var newlink, newspan;
    try {
        newlink = document.createElement('a');
        newlink.setAttribute('href', 'https://gate.eveonline.com/Mail/ComposeForward/352241208');
        newlink.setAttribute('target', 'blank');
        newspan = document.createElement('span');
        newspan.innerHTML = 'Recruitment Mail';

        newlink.appendChild(newspan);
        document.getElementsByClassName('buttonsContainer')[0].appendChild(newlink);
    }
    catch (ex) {
        //alert(ex.message);
    }
}

function EditRecruitmentMail() {
    var text;
    var index;
    var searchText = "To: ";

    try {
        text = document.getElementById("subject").value.replace("Fw: ", "");
        document.getElementById("subject").value = text;

        for (var i = 0; i < 8; i++) {
            document.getElementById('ql-editor-1').removeChild(
                document.getElementById('ql-editor-1').childNodes[0]
                );
        }

        //index = document.getElementById("mailContents").value.indexOf(searchText) + searchText.length;
        //text = document.getElementById("mailContents").value.substring(index, document.getElementById("mailContents").value.length);
        //document.getElementById("mailContents").value = text;
    }
    catch (ex) {
        alert(ex.message);
    }
}