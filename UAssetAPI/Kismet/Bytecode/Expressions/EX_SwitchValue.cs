﻿namespace UAssetAPI.Kismet.Bytecode.Expressions
{
    /// <summary>
    /// Represents a case in a Kismet bytecode switch statement.
    /// </summary>
    public struct FKismetSwitchCase
    {
        /// <summary>
        /// The index value term of this case.
        /// </summary>
        public Expression CaseIndexValueTerm;

        /// <summary>
        /// Code offset to the next case.
        /// </summary>
        public uint NextOffset;

        /// <summary>
        /// The main case term.
        /// </summary>
        public Expression CaseTerm;

        public FKismetSwitchCase(Expression caseIndexValueTerm, uint nextOffset, Expression caseTerm)
        {
            CaseIndexValueTerm = caseIndexValueTerm;
            NextOffset = nextOffset;
            CaseTerm = caseTerm;
        }
    }

    /// <summary>
    /// A single Kismet bytecode instruction, corresponding to the <see cref="EExprToken.EX_SwitchValue"/> instruction.
    /// </summary>
    public class EX_SwitchValue : Expression
    {
        /// <summary>
        /// The token of this expression.
        /// </summary>
        public override EExprToken Token { get { return EExprToken.EX_SwitchValue; } }

        /// <summary>
        /// Code offset to jump to when finished.
        /// </summary>
        public uint EndGotoOffset;

        /// <summary>
        /// The index term of this switch statement.
        /// </summary>
        public Expression IndexTerm;

        /// <summary>
        /// The default term of this switch statement.
        /// </summary>
        public Expression DefaultTerm;

        /// <summary>
        /// All the cases in this switch statement.
        /// </summary>
        public FKismetSwitchCase[] Cases;

        public EX_SwitchValue()
        {

        }

        /// <summary>
        /// Reads out the expression from a BinaryReader.
        /// </summary>
        /// <param name="reader">The BinaryReader to read from.</param>
        public override void Read(AssetBinaryReader reader)
        {
            ushort numCases = reader.ReadUInt16(); // number of cases, without default one
            EndGotoOffset = reader.ReadUInt32();
            IndexTerm = ExpressionSerializer.ReadExpression(reader);

            Cases = new FKismetSwitchCase[numCases];
            for (int i = 0; i < numCases; i++)
            {
                Expression termA = ExpressionSerializer.ReadExpression(reader);
                uint termB = reader.ReadUInt32();
                Expression termC = ExpressionSerializer.ReadExpression(reader);
                Cases[i] = new FKismetSwitchCase(termA, termB, termC);
            }

            DefaultTerm = ExpressionSerializer.ReadExpression(reader);
        }

        /// <summary>
        /// Writes the expression to a BinaryWriter.
        /// </summary>
        /// <param name="writer">The BinaryWriter to write from.</param>
        /// <returns>The length in bytes of the data that was written.</returns>
        public override int Write(AssetBinaryWriter writer)
        {
            writer.Write((ushort)Cases.Length);
            writer.Write(EndGotoOffset);
            ExpressionSerializer.WriteExpression(IndexTerm, writer);
            for (int i = 0; i < Cases.Length; i++)
            {
                ExpressionSerializer.WriteExpression(Cases[i].CaseIndexValueTerm, writer);
                writer.Write(Cases[i].NextOffset);
                ExpressionSerializer.WriteExpression(Cases[i].CaseTerm, writer);
            }
            ExpressionSerializer.WriteExpression(DefaultTerm, writer);
            return 0;
        }
    }
}
