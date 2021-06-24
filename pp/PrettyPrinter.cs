using System;

namespace Reko.Core.Output
{
    public class PrettyPrinter
    {
        // overloading some ASCII codes, which are unlikely to occur in real text docs
        private const char INDENT = '\x02';     // STX
        private const char OUTDENT = '\x03';    // ETX
        private const char NEWLINE = '\n';
        private const char MARKER = '\v';       // VT
        private const char ESCAPE = '%';

        private int indentWidth = 4;

        private int device_output_width = 80;
        private OutputDevice output;

        public PrettyPrinter(OutputDevice output)
        {
            this.output = output;
            break_dq = new();
            buffer = new();
            device_output_width = output.DeviceWidth;
            indentWidth = output.IndentWidth;
        }

        private Dequeue<(int chars_enqueued, int level, bool connected)> break_dq;
        private Dequeue<(char character, int level)> buffer;
        private int current_level;
        private int break_level;
        private int total_chars_enqueued;
        private int total_chars_flushed;
        private int total_pchars_flushed;
        private int total_pchars_enqueued;

        public void prettyprint(string s)
        {
            for (int i = 0; i < s.Length; ++i)
            {
                if (s[i] == ESCAPE && i < s.Length - 1)
                {
                    ++i;
                    switch (s[i])
                    {
                    case '{': BeginGroup(); break;
                    case '}': EndGroup(); break;
                    case 't': Indent(); break;
                    case 'b': Outdent(); break;
                    case 'n': UnconditionalLineBreak(); break;
                    case 'o': OptionalLineBreak(); break;
                    case 'c': ConnectedLineBreak(); break;
                    default: AddPrintableCharacter(s[i]); break;
                    }
                }
                else if (s[i] == '\r' || s[i] == '\n')
                {
                    // Ignored, use %n instead
                }
                else
                {
                    AddPrintableCharacter(s[i]);
                }
            }
            Flush();
        }

        private void BeginGroup()
        {
            ++current_level;
        }

        private void EndGroup()
        {
            --current_level;
            if (break_level > current_level)
                break_level = current_level;
        }

        private void Indent()
        {
            buffer.PushBack((INDENT, int.MaxValue));
            ++total_chars_enqueued;
        }

        private void Outdent()
        {
            buffer.PushBack((OUTDENT, int.MaxValue));
            ++total_chars_enqueued;
        }

        private void UnconditionalLineBreak()
        {
            break_dq.Clear();
            break_level = current_level;
            buffer.PushBack((NEWLINE, int.MaxValue));
            ++total_chars_enqueued;
            print_buffer(buffer.Count);
        }

        private void Flush()
        {
            break_dq.Clear();
            break_level = current_level;
            print_buffer(buffer.Count);
        }
        private void OptionalLineBreak()
        {
            // discard breaks we are no longer interested in
            while (break_dq.Count > 0 &&
                (break_dq.Front.level > current_level
                    || break_dq.Front.level == current_level
                    && !break_dq.Front.connected))
            {
                break_dq.PopFront();
            }
            break_dq.PushFront((total_chars_enqueued, current_level, false));
        }

        private void ConnectedLineBreak()
        {
            if (break_level < current_level)
            {
                // discard breaks we are no longer interested in
                while (break_dq.Count > 0 &&
                    break_dq.Front.level >= current_level)
                {
                    break_dq.PopFront();
                }
                buffer.PushBack((MARKER, current_level));
                ++total_chars_enqueued;
                break_dq.PushFront((total_chars_enqueued, current_level, true));
            }
            else
            {
                // take an immediate line break, break_level = current_level
                break_dq.Clear();
                buffer.PushBack((NEWLINE, int.MaxValue));
                ++total_chars_enqueued;
                print_buffer(buffer.Count);
            }
        }

        private void AddPrintableCharacter(char ch)
        {
            // it is a printable character
            int enqueued_chars = total_pchars_enqueued - total_pchars_flushed;
            if (enqueued_chars + output.LeftMargin >= output.DeviceWidth)
            {
                // must split line
                if (break_dq.Count > 0)
                {
                    // split line at a break
                    var temp = break_dq.PopBack();
                    break_level = temp.level;
                    print_buffer(temp.chars_enqueued - total_chars_flushed);
                    if (!temp.connected)
                        output.WriteLine();
                    break_level = Math.Min(break_level, current_level);
                }
                else
                {
                    // there are no breaks to take
                }
            }
            // put the current character into the buffer
            buffer.PushBack((ch, int.MaxValue));
            ++total_chars_enqueued;
            ++total_pchars_enqueued;
        }


        /// <summary>
        /// Send the k leftmost symbols in the buffer to the output device. 
        /// </summary>
        private void print_buffer(int k)
        {
            for (int i = 0; i < k; ++i)
            {
                var temp = buffer.PopFront();
                ++total_chars_flushed;
                switch (temp.character)
                {
                case MARKER:
                    if (temp.level <= break_level) output.WriteLine();
                    break;
                case NEWLINE:
                    output.WriteLine();
                    break;
                case INDENT:
                    output.Indent();
                    break;
                case OUTDENT:
                    output.Outdent();
                    break;
                default:
                    ++total_pchars_flushed;
                    output.Write(temp.character);
                    break;
                }
            }
        }
    }
}