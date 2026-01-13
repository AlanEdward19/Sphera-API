using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Sphera.API.Billing.BilletConfigurations;
using Sphera.API.Billing.Billets;

namespace Sphera.API.Billing;

public class BradescoFileGenerator
{
    public static Stream GenerateRemmitanceFile(List<Billet> billets, int remittanceSequentialNumber)
    {
        string data = "";

        var billetConfiguration = billets.First().Configuration;
        data += GenerateHeader(billetConfiguration, remittanceSequentialNumber) + "\r\n";

        var registerSequentialNumber = 1;
        for (var i = 0; i < billets.Count; i++)
        {
            registerSequentialNumber++;
            var billet = billets[i];
            data += GenerateTitle(billet, billetConfiguration, registerSequentialNumber) + "\r\n";
        }
        registerSequentialNumber++;
        
        data += GenerateTrailer(registerSequentialNumber) + "\r\n";
        
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(data);
        writer.Flush();
        stream.Position = 0;

        return stream;
    }
    
    public static string Text(object input, int length, char padding = ' ', bool padRight = false)
    {
        var strInput = input?.ToString() ?? string.Empty;
        strInput = Regex.Replace(strInput, "á|à|ã|â|ä", "a");
        strInput = Regex.Replace(strInput, "é|è|ê|ë", "e");
        strInput = Regex.Replace(strInput, "í|ì|î|ï", "i");
        strInput = Regex.Replace(strInput, "ó|ò|õ|ô|ö", "o");
        strInput = Regex.Replace(strInput, "ú|ù|û|ü", "u");
        strInput = Regex.Replace(strInput, "ç", "c");
        strInput = Regex.Replace(strInput, "Á|À|Ã|Â|Ä", "A");
        strInput = Regex.Replace(strInput, "É|È|Ê|Ë", "E");
        strInput = Regex.Replace(strInput, "Í|Ì|Î|Ï", "I");
        strInput = Regex.Replace(strInput, "Ó|Ò|Õ|Ô|Ö", "O");
        strInput = Regex.Replace(strInput, "Ú|Ù|Û|Ü", "U");
        strInput = Regex.Replace(strInput, "Ç", "C");
        
        if (strInput.Length > length)
            return strInput.Substring(0, length);
        return padRight ? strInput.PadRight(length, padding) : strInput.PadLeft(length, padding);
    }

    public static string GenerateHeader(BilletConfiguration billetConfiguration, int remittanceSequentialNumber)
    {
        var sb = new StringBuilder();
        
        // 001 Identificação do Registro
        sb.Append("0");
        
        // 002 Identificação do Arquivo-Remessa
        sb.Append("1");
        
        // 003-009 Literal Remessa
        sb.Append("REMESSA");
        
        // 010-011 Código de Serviço
        sb.Append("01");
        
        // 012-026 Literal Serviço
        sb.Append(Text("COBRANCA", 15, padRight: true));
        
        // 027-046 Código da Empresa
        sb.Append(Text(billetConfiguration.CompanyCode, 20, '0'));                
        
        // 047-076 Nome da Empresa
        sb.Append(Text(billetConfiguration.CompanyName, 30, padRight: true));
        
        // 077-079 Número do Bradesco na Câmara de Compensação
        sb.Append("237");
        
        // 080-094 Nome do Banco por Extenso
        sb.Append(Text("BRADESCO", 15, padRight: true)); 
        
        // 095-100 Data da Gravação do Arquivo
        sb.Append(DateTime.Now.ToString("ddMMyy"));
        
        // 101-108 Branco
        sb.Append(Text("", 8));
        
        // 109-110 Identificação do Sistema
        sb.Append("MX");
        
        // 111-117 Nº Sequencial de Remessa 
        sb.Append(Text(remittanceSequentialNumber, 7, '0'));
        
        // 118-394 Branco
        sb.Append(Text("", 277));
        
        // 395-400 Nº Sequencial do Registro de Um em Um
        sb.Append("000001");
        
        return sb.ToString();
    }

    public static string GenerateTitle(Billet billet, BilletConfiguration billetConfiguration, int sequentialNumber)
    {
        var sb = new StringBuilder();

        var realFormattingConfigurations = new NumberFormatInfo
            { CurrencyDecimalDigits = 2, CurrencyDecimalSeparator = ".", CurrencySymbol = "" };
        
        // 00
        sb.Append("1");
        
        // 002-006 Código da Agência do Pagador Exclusivo para Débito em Conta (opcional)
        sb.Append("0".PadRight(5, '0'));

        // 007 Dígito da Agência de Débito (opcional)
        sb.Append("0");
        
        // 008-012 Razão da Conta-Corrente (opcional)
        sb.Append("0".PadRight(5, '0'));
        
        // 013-019 Conta-Corrente (opcional)
        sb.Append("0".PadRight(7, '0'));

        // 020 Dígito da Conta-Corrente (opcional) 
        sb.Append("0");

        // 021-037 Identificação da Empresa Beneficiária no Banco - Zero, Carteira, Agência e Conta - Corrente  
        sb.Append(Text($"0{billetConfiguration.WalletNumber}{billetConfiguration.AgencyNumber}{billetConfiguration.AccountNumber}{billetConfiguration.AccountDigit}", 17));
        
        // 038-062 Nº Controle do Participante
        sb.Append(Text("", 25));
        
        // 063-065 Código do Banco a ser debitado na Câmara de Compensação 
        sb.Append("237");
        
        // 066 Campo de Multa 
        sb.Append(billetConfiguration.HasFine ? "2" : "0");
        
        // 067-070 Percentual de Multa 
        sb.Append(Text(billetConfiguration.HasFine      
            ? (billetConfiguration.FinePercentage!.Value * 100).ToString("0000").Replace(".", "")
            : "0000", 4, '0'));
        
        // TODO: decifrar dps
        // 071-081 Identificação do Título no Banco
        sb.Append("00000000000");
        
        // 082 Dígito de Autoconferência do Número Bancário 
        sb.Append("0");
        
        // 083-092 Desconto Bonificação por dia
        sb.Append(Text(billetConfiguration.DailyDiscount.ToString("C", realFormattingConfigurations ).Replace(".", ""), 10, '0'));

        // 093 Condição para Emissão da Papeleta de Cobrança 
        sb.Append("2");
        
        // 094 Ident. se emite Boleto  para Débito Automático
        sb.Append("N");
        
        // 095-104 Identificação da Operação do Banco 
        sb.Append(Text("", 10));
        
        // 105 Indicador Rateio Crédito (opcional)
        sb.Append(" ");                
        
        // 106 Endereçamento para Aviso do Débito Automático em Conta Corrente (opcional)
        sb.Append("2");
        
        // 107-108 Quantidade de Pagamentos 
        sb.Append("  ");                   
        
        // 109-110 Identificação da Ocorrência
        sb.Append("01");                      
        
        // 111-120 Nº do Documento 
        sb.Append(Text("", 10, padRight: true));
        
        // 121-126 Data do Vencimento do Título
        sb.Append(billet.Installment.DueDate.ToString("ddMMyy"));                             
        
        // 127-139 Valor do Título 
        sb.Append(Text(billet.Installment.Amount.ToString("C", realFormattingConfigurations ), 13, '0'));
        
        // 140-142 Banco Encarregado da Cobrança 
        sb.Append("000");
        
        // 143-147 Agência Depositária
        sb.Append("00000");
        
        // 148-149 Espécie de Título 
        sb.Append("01");            
        
        // 150 Identificação
        sb.Append("N");
        
        // 151-156 Data da Emissão do Título
        sb.Append(billet.Installment.Invoice.IssueDate.ToString("ddMMyy"));
        
        // 157-160 1ª / 2ª Instrução
        sb.Append("0000");                              
        
        // 161-173 Valor a ser Cobrado por Dia de Atraso
        sb.Append(Text(billetConfiguration.DailyInterest.ToString("C", realFormattingConfigurations ).Replace(".", ""), 13, '0'));
        
        // 174-179 Data Limite P/Concessão de Desconto
        sb.Append(billetConfiguration.DiscountLimitDate.ToString("ddMMyy"));
        
        // 180-192 Valor do Desconto
        sb.Append(Text(billetConfiguration.DiscountAmount.ToString("C", realFormattingConfigurations ).Replace(".", ""), 13, '0'));
        
        // 193-205 Valor do IOF
        sb.Append("0000000000000");
        
        // 206-218 Valor do Abatimento
        sb.Append(Text(billetConfiguration.RebateAmount.ToString("C", realFormattingConfigurations ).Replace(".", ""), 13, '0'));
        
        // 219-220 Identificação do Pagador
        sb.Append("02");
        
        // 221-234 Nº Inscrição do Pagador
        var client = billet.Client;
        sb.Append(Text(client.Cnpj, 14, '0'));
        
        // 235-274 Nome do Pagador
        sb.Append(Text(client.LegalName, 40, padRight: true));
        
        // 275-314 Endereço do Pagador
        sb.Append(Text($"{client.Address.Street}, {client.Address.Number} - {client.Address.City}, {client.Address.State}", 40, padRight: true));
        
        // 315-326 1ª Mensagem
        sb.Append(Text(billetConfiguration.FirstMessage, 12, padRight: true));
        
        // 327-331 CEP do Pagador
        sb.Append(Text(client.Address.ZipCode, 8, '0'));
        
        // 335-394 2ª Mensagem
        sb.Append(Text(billetConfiguration.SecondMessage, 60, padRight: true));
        
        // 395-400 Nº Sequencial do Registro de Um em Um
        sb.Append(Text(sequentialNumber, 6, '0'));
        
        return sb.ToString();
    }
    
    public static string GenerateTrailer(int sequentialNumber)
    {
        var sb = new StringBuilder();

        // 001 Identificação do Registro
        sb.Append("9");
        
        // 002-394 Branco
        sb.Append(Text("", 393));
        
        // 395-400 Número Sequencial de Registro 
        sb.Append(Text(sequentialNumber, 6, '0'));
        
        return sb.ToString();
    }
}