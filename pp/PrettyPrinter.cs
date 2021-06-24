using System;

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
        this.device_output_width = output.DeviceWidth;
        this.indentWidth = output.IndentWidth;
    }

    private Dequeue<(int chars_enqueued, int level, bool connected)> break_dq;
    private Dequeue<(char character, int level)> buffer;
    private int current_level;
    private int break_level;
    private int total_chars_enqueued;
    private int total_chars_flushed;
    private int total_pchars_flushed;
    private int total_pchars_enqueued;

    public void prettyprint(string s) {
        for (int i = 0; i < s.Length; ++i)
        {
            if (s[i] == ESCAPE && i < s.Length - 1)
            {
                ++i;
                switch (s[i])
                {
                case '{': /* start group */
                    ++current_level;
                    break;
                case '}': /* end group */
                    current_level = current_level - 1;
                    if (break_level > current_level)
                        break_level = current_level;
                    break;
                case 't': // indent
                    buffer.right_enqueue((INDENT, int.MaxValue));
                    ++total_chars_enqueued;
                    break;
                case 'b': // outdent
                    buffer.right_enqueue((OUTDENT, int.MaxValue));
                    ++total_chars_enqueued;
                    break;
                case 'n': // unconditional line break
                    break_dq.Clear();
                    break_level = current_level;
                    buffer.right_enqueue((NEWLINE, int.MaxValue));
                    ++total_chars_enqueued;
                    print_buffer(buffer.Count);
                    break;
                case 'o': // optional line break
                    while (break_dq.Count > 0 &&
                        (break_dq.Left.level > current_level
                            || (break_dq.Left.level == current_level
                            && !break_dq.Left.connected)))
                    { // discard breaks we are no longer interested in
                        break_dq.left_dequeue();
                    }
                    break_dq.left_enqueue((total_chars_enqueued, current_level, false));
                    break;
                case 'c': // connected line break 
                    if (break_level < current_level)
                    {
                        // discard breaks we are no longer interested in
                        while (break_dq.Count > 0 &&
                            break_dq.Left.level >= current_level) 
                        {
                            break_dq.left_dequeue();
                        }
                        buffer.right_enqueue((MARKER, current_level));
                        ++total_chars_enqueued;
                        break_dq.left_enqueue((total_chars_enqueued, current_level, true));
                    }
                    else
                    { 
                        // take an immediate line break, break_level = current_level
                        break_dq.Clear();
                        buffer.right_enqueue((NEWLINE, int.MaxValue));
                        ++total_chars_enqueued;
                        print_buffer(buffer.Count);
                    }
                    break;
                default:
                    AddPrintableCharacter(s[i]);
                    break;
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
    }   /* prettyprint */

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
                var temp = break_dq.right_dequeue();
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
        buffer.right_enqueue((ch, int.MaxValue));
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
            var temp = buffer.left_dequeue();
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
