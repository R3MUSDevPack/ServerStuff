// ==UserScript==
// @name         	R3MUS Mailing List Copy Enabler
// @namespace     	http://jaynestown.rocks
// @description	Enables the mailing list box for copying
// @include	https://gate.eveonline.com/Mail/ComposeForward/*
// @include	https://gate.eveonline.com/Mail/Compose?tag=r3mus
// @include	https://gate.eveonline.com/Mail/MailingList/145182300
// @version       	0.0.1.5
// @grant 	none
// ==/UserScript==

var textClass = 'eveFontffffff eveFontSize14';
var paraHeadClass = 'eveFont007fff eveFontSize18';
var mailHeadClass = 'eveFont007fff eveFontSize24';
var redThisClass = 'eveFontff0000 eveFontSize14';
var linkClass = 'eveFontffa600 eveFontSize14';

//if (document.URL.indexOf('ComposeForward/353295520') > -1) {
if (document.URL.indexOf('Compose?tag=r3mus') > -1) {
    EditRecruitmentMail();
}
else if (document.URL.indexOf('MailingList/145182300') > -1) {
    AddLinkToRecruitmentMail();
}

function AddLinkToRecruitmentMail() {
    var newlink, newspan;
    try {
        newlink = document.createElement('a');
        //newlink.setAttribute('href', 'https://gate.eveonline.com/Mail/ComposeForward/353295520');
        newlink.setAttribute('href', 'https://gate.eveonline.com/Mail/Compose?tag=r3mus');
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
        //document.getElementById("subject").value = text;
        document.getElementById("subject").value = 'Like Explosions?';

        //for (var i = 0; i < 8; i++) {
        //    document.getElementById('ql-editor-1').removeChild(
        //        document.getElementById('ql-editor-1').childNodes[0]
        //        );
        //}

        //document.getElementById('basic-editor').removeChild(document.getElementById('basic-editor').childNodes[0]);
        document.getElementById('ql-editor-1').removeChild(document.getElementById('ql-editor-1').childNodes[0]);

        //document.getElementById('basic-editor').appendChild(fixMail());
        document.getElementById('ql-editor-1').appendChild(fixMail());

        //index = document.getElementById("mailContents").value.indexOf(searchText) + searchText.length;
        //text = document.getElementById("mailContents").value.substring(index, document.getElementById("mailContents").value.length);
        //document.getElementById("mailContents").value = text;
    }
    catch (ex) {
        alert(ex.message);
    }
}

function fixMail() {

    var para = document.createElement('p');
    para.appendChild(createHoldingDiv([createGreeting('Hey there!')]));
    para.appendChild(createNewLine());
    para.appendChild(createHoldingDiv([createTextSpan("As you're in an NPC corp I hope you might consider us for your future career in EVE.")]));

    para.appendChild(createHoldingDiv([createParagraphHead("Who we are")]));
    para.appendChild(createHoldingDiv([redThisUp("Raised By Wolves Inc"), createTextSpan(" is a PVP-centric corporation that lives in sov null space which welcomes both new and new to PVP players as well as more experienced combat pilots.")]));

    para.appendChild(createHoldingDiv([stripBreaks(createTextSpan("We are proud members of the "), 2), redThisUp("NAGA alliance and the RED Menace coalition"), createTextSpan(", and need your help to support our alliance and coalition brothers and sisters in one of the most active and fun regions in the game.")]));

    para.appendChild(createHoldingDiv([createParagraphHead("What we can offer")]));
    para.appendChild(createHoldingDiv([stripBreaks(createTextSpan("Our focus is on building combat-capable pilots who are able to have fun PVPing but importantly a place where you are treated as a member of a community rather than just a number in a fleet. There are great income opportunities in our space to fund your PvP habit and you will get "), 2),
    redThisUp("free ships and skill books"), createTextSpan(" so you can take part in fleet combat without worrying about the cost.")]));


    para.appendChild(createHoldingDiv([createTextSpan("We have web based courses and live training sessions that you can take to teach you the skills you need to join in on fleets and are committed to helping you to get the most out of your EVE experience.")]));

    para.appendChild(createHoldingDiv([stripBreaks(createTextSpan("For vets, our alliance has "), 2), redThisUp("very generous ship replacement and cheap capital programmes"),
    createTextSpan(" because we feel that all players should be able to have fun shooting at spaceships without worrying about the cost.")]));

    para.appendChild(createHoldingDiv([createParagraphHead("What we're looking for")]));
    para.appendChild(createHoldingDiv([redThisUp("Our corp values maturity, patience and intelligence"), createTextSpan(". We want people who will work as a team so that everyone can have fun doing what we enjoy the most (hint: it involves explosions!) so we like you to join in on important ops whenever you can. We do need you to be able to speak English fluently, to have Teamspeak and a working microphone and to want to be part of a fantastic supportive community. We also need a full API key for the quick application process but trial accounts are welcome (we're sure you'll upgrade!).")
    ]));

    para.appendChild(createHoldingDiv([stripBreaks(createTextSpan("Head on over to "), 2),
    createHyperlink("http://www.r3mus.org/recruitment/apply", "http://www.r3mus.org/recruitment/apply"),
    createTextSpan(" and submit an application and we will get right on it.")
    ]));

    para.appendChild(createHoldingDiv([createGreeting('Fly safe!')]));
    para.appendChild(createNewLine());
    para.appendChild(createHoldingDiv([createGreeting('Raised By Wolves Inc')]));

    return para;
}

function stripBreaks(object, count) {
    for (var i = 0; i < count; i++) {
        object.removeChild(object.childNodes[object.childNodes.length - 1]);
    }
    return object;
}

function createHyperlink(href, text) {
    var span = document.createElement('span');
    span.className = linkClass;
    var link = document.createElement('a');
    link.href = href;
    var textnode = document.createTextNode(text);
    link.appendChild(textnode);
    span.appendChild(link);
    return span;
}

function redThisUp(text) {
    var span = createTextSpan(text);
    span.className = redThisClass;
    span = stripBreaks(span, 2);
    return span;
}

function createParagraphHead(text) {
    var span = document.createElement('span');
    span.className = paraHeadClass;
    var node = document.createElement('b');
    var textnode = document.createTextNode(text);
    node.appendChild(textnode);
    span.appendChild(node);
    var br = document.createElement('br');
    span.appendChild(br);
    return span;
}

function createTextSpan(text) {
    var span = document.createElement('span');
    span.className = textClass;
    var textnode = document.createTextNode(text);
    span.appendChild(textnode);
    var br = document.createElement('br');
    span.appendChild(br);
    br = document.createElement('br');
    span.appendChild(br);
    return span;
}

function createNewLine() {
    var div = document.createElement('div');
    var br = document.createElement('br');
    div.appendChild(br);
    return div;
}

function createGreeting(title) {
    var span = document.createElement('span');
    span.className = mailHeadClass;
    var node = document.createElement('b');
    var textnode = document.createTextNode(title);
    node.appendChild(textnode);
    span.appendChild(node);
    return span;
}

function createHoldingDiv(contents) {
    var div = document.createElement('div');
    for (var i = 0; i < contents.length; i++) {
        div.appendChild(contents[i]);
    }
    return div;
}