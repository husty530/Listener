///
/// Web Speech API
///

let _app = null;
let _autoRestart = null;
const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition
const SpeechGrammarList = window.SpeechGrammarList || window.webkitSpeechGrammarList
const SpeechRecognitionEvent = window.SpeechRecognitionEvent || window.webkitSpeechRecognitionEvent
const _speechRecognitionList = new SpeechGrammarList();
const _recognition = new SpeechRecognition();

// your project name, ja or en-US, string array, bool
function startWebSpeech(appName, lang = 'ja', autoRestart = true, dictionary = null) {
    _app = appName;
    _autoRestart = autoRestart;
    _recognition.lang = lang;
    if (dictionary != null) {
        if (lang == 'ja')
            _speechRecognitionList.addFromString('#JSGF V1.0 JIS ja; grammar words; public <word> = ' + dictionary.join(' | ') + ' ;', 1);
        else
            _speechRecognitionList.addFromString('#JSGF V1.0; grammar words; public <word> = ' + dictionary.join(' | ') + ' ;', 1);
        _recognition.grammars = _speechRecognitionList;
    }
    _recognition.continuous = false;
    _recognition.interimResults = false;
    _recognition.maxAlternatives = 1;
    if (_autoRestart)
        _recognition.start();
}

function start() {
    _recognition.start();
}

function stop() {
    _recognition.stop();
}

_recognition.onspeechend = () => {
    _recognition.stop();
}

_recognition.onresult = async e => {
    const msg = e.results[0][0].transcript;
    await DotNet.invokeMethodAsync(_app, 'NotifySpeechResult', msg);
    console.log(msg);
}

_recognition.onend = () => {
    if (_autoRestart)
        _recognition.start();
}