using System;
using System.Threading.Tasks;
using System.Reactive.Subjects;
using Microsoft.JSInterop;

namespace Listener.Pages
{
    partial class Index
    {

        private static string _text = "";
        private static string _positionLeft = "20%";
        private static string _positionTop = "20%";
        private static readonly string[] _gainKeys = new string[] { "ちょっと", "かなり" };
        private static readonly string[] _directionKeys = new string[] { "上に", "下に", "左に", "右に" };
        private static readonly Subject<bool> _ = new();

        public Index()
        {
            _text = "";
            Task.Run(async () =>
            {
                await Task.Run(() => { while (Js is null) ; });
                await Js.InvokeVoidAsync("startWebSpeech", "Listener");
            });
            _.Subscribe(_ => InvokeAsync(() => StateHasChanged()));
        }

        [JSInvokable]
        public static void NotifySpeechResult(string text)
        {
            _text = text;
            Interpret(text);
            _.OnNext(true);
        }

        private static void Interpret(string text)
        {
            var gain = 8;
            for (int i = 0; i < _gainKeys.Length; i++)
                if (text.Contains(_gainKeys[i]))
                    gain -= (int)Math.Pow(-1, i) * 5;
            var vertical = 0;
            var horizonal = 0;
            for (int i = 0; i < _directionKeys.Length; i++)
                if (text.Contains(_directionKeys[i]))
                {
                    if (i < 2)
                        vertical -= (int)Math.Pow(-1, i);
                    else
                        horizonal -= (int)Math.Pow(-1, i);
                }
            if (int.TryParse(_positionLeft.Replace("%", ""), out var left) && int.TryParse(_positionTop.Replace("%", ""), out var top))
            { 
                _positionLeft = $"{left + gain * horizonal}%";
                _positionTop = $"{top + gain * vertical}%";
            }
        }

    }
}
