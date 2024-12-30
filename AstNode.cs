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

public class AstParameterInt64 : AstNode
{
    private string identifier_;

    public AstParameterInt64(string identifier)
    {
        identifier_ = identifier;
    }
}

public class AstDeclarationInt64 : AstNode
{
    private string identifier_;
    private Int64 initial_value_;

    public AstDeclarationInt64(string identifier, Int64 initial_value = 0)
    {
        identifier_ = identifier;
        initial_value_ = initial_value;
    }
}

public class AstLiteralInt64 : AstNode
{
    public Int64 value_ { get; private set; }

    public AstLiteralInt64(Int64 value)
    {
        value_ = value;
    }
}

public class AstVariableInt64 : AstNode
{
    public string identifier_ { get; private set; }
    public AstVariableInt64(string identifier_)
    {
        this.identifier_ = identifier_;
    }
}

public class AstAssignment : AstNode
{
    public AstNode variable_ { get; private set; }
    public AstNode assign_value_ { get; private set; }

    public AstAssignment(AstNode variable, AstNode assign_value)
    {
        variable_ = variable;
        assign_value_ = assign_value;
    }
}

public class AstReturn : AstNode
{
    public AstNode return_value_ { get; private set; }

    public AstReturn(AstNode return_value_)
    {
        this.return_value_ = return_value_;
    }
}

public class AstBlock : AstNode
{
    public List<AstNode> statements_ { get; private set; }
    
    public AstBlock()
    {
        statements_ = new List<AstNode>();
    }

    public void AddStatement(AstNode node)
    {
        statements_.Add(node);
    }
}

public class AstProcedure : AstNode
{
    public string signature_ { get; private set; }
    public List<AstNode> arguments_ { get; private set; }
    public AstBlock block_ { get; private set; }

    // @TODO : Shouldn't be string!!!
    public string return_type_ { get; private set; }
    
    public AstProcedure(string signature)
    {
        signature_ = signature;
        arguments_ = new List<AstNode>();
        block_ = new AstBlock();
        return_type_ = "";
    }

    public void AddParameter(AstParameterInt64 param)
    {
        arguments_.Add(param);
    }

    public void SetReturnType(string return_type)
    {
        return_type_ = return_type;
    }

    public void SetBlock(AstBlock block)
    {
        block_ = block;
    }
}