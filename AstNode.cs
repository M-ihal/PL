using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AstNode
{
    public AstNode()
    {
    }
}

public class AstProgramRoot : AstNode
{
    private List<AstNode> sub_nodes_;
    public IEnumerable<AstNode> SubNodes { get { return sub_nodes_; } }

    public AstProgramRoot()
    {
        sub_nodes_ = new List<AstNode>();
    }

    public void AddSubNode(AstNode node)
    {
        sub_nodes_.Add(node);
    }
}

public class AstTypeInt64 : AstNode
{
    string identifier_;
    int bytes_;

    public AstTypeInt64(string identifier)
    {
        identifier_ = identifier;
        bytes_ = 8;
    }
}

public class AstProcedure : AstNode
{
    string signature_;
    List<AstNode> arguments_;

    public AstProcedure(string singnature)
    {
        signature_ = singnature;
        arguments_ = new List<AstNode>();
    }

    public void AddArgument(string identifier_, TokenType type)
    {
        switch (type)
        {
            default:
            {
                throw new Exception("Undefined argument type.");
            }

            case TokenType.KEYWORD_INT64:
            {
                AstTypeInt64 ast_int64 = new AstTypeInt64(identifier_);
                arguments_.Add(ast_int64);
            } break;
        }
    }
}