using System;
using System.Text;
using ITOrm.Core.Helper;

namespace ITOrm.Core.Utility.Json
{

    internal class ValueJsonParser : JsonParserBase {

        protected StringBuilder sb = new StringBuilder();
        private static readonly String separatorChars = ",\":[]{}";

        private Object _result;

        public override Object getResult() {
            return _result;
        }

        public ValueJsonParser() {
        }

        public ValueJsonParser( CharSource charSrc )
            : base( charSrc ) {
        }

        protected override void parse() {

            paserOne();

            charSrc.back();

            String s = sb.ToString().Trim();
            if (s.Equals( "" )) throw ex( "value is empty" );


            _result = getStringValue( s );
        }


        private void paserOne() {

            char c = charSrc.getCurrent();

            if (c >= ' ' && separatorChars.IndexOf( c ) < 0) {

                if (c == '\\') {
                    processEscape();
                }
                else
                    sb.Append( c );

                if (charSrc.isEnd()) return;

                charSrc.move();

                paserOne();
            }
        }

        protected void processEscape() {

            charSrc.move();

            char c = charSrc.getCurrent();

            if (c == 'b')
                sb.Append( '\b' );
            else if (c == 't')
                sb.Append( '\t' );
            else if (c == 'n')
                sb.Append( '\n' );
            else if (c == 'n')
                sb.Append( '\n' );
            else if (c == 'r')
                sb.Append( '\r' );
            else if (c == 'u') {
                //sb.Append( '' ); // TODO 
            }
            else if (c == '"' || c == '\'' || c == '/' || c == '\\')
                sb.Append( c );
            else
                throw ex( "not a valid escape character" );
        }

        private static Object getStringValue( String s ) {

            if (Util.IsInteger(s)) return Util.StringToInt(s,-1);
            if (Util.IsDecimal(s)) return Util.StringToDecimal(s);
            if (Util.IsBoolean(s)) return Util.StringToBool(s);
            return s;
        }

    }

}
