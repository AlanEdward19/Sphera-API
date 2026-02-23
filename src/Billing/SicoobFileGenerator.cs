using System.Text;
using System.Text.RegularExpressions;
using BoletoNetCore;
using HtmlToPDFCore;
using Sphera.API.Billing.BilletConfigurations;
using Sphera.API.Billing.Billets;

namespace Sphera.API.Billing;

public class SicoobFileGenerator
{
    public static Stream GenerateRemmitanceFile(List<Billet> billets, int remittanceSequentialNumber, int nossoNumero)
    {
        var billetConfiguration = billets.First().Configuration;
        
        var banco = Banco.Instancia(756);
        
        banco.Beneficiario = new Beneficiario
        {
            Codigo = billetConfiguration.CompanyCode,
            Nome = billetConfiguration.CompanyName,
            CPFCNPJ = billetConfiguration.CompanyId,
            ContaBancaria = new ContaBancaria
            {
                Agencia = Text(billetConfiguration.AgencyNumber, 4, '0'),
                DigitoAgencia = "",
                Conta = billetConfiguration.AccountNumber,
                DigitoConta = billetConfiguration.AccountDigit,
                CarteiraPadrao = billetConfiguration.WalletNumber
            },
        };
        
        var boletos = new Boletos
        {
            Banco = banco
        };
        for (var i = 0; i < billets.Count; i++)
        {
            
            var billet = billets[i];
            var boleto = GenerateBillet(billet, billetConfiguration, nossoNumero, banco);
            
            billets[i].NossoNumero = nossoNumero;
            boletos.Add(boleto);
            nossoNumero++;
        }

        var stream = new MemoryStream();
        var arquivoRemessa = new ArquivoRemessa(boletos.Banco, TipoArquivo.CNAB400, 1);

        arquivoRemessa.NumeroArquivoRemessa = remittanceSequentialNumber;
        arquivoRemessa.GerarArquivoRemessa(boletos, stream, false);
        
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
    
    public static Boleto GenerateBillet(Billet billet, BilletConfiguration billetConfiguration, int nossoNumero, IBanco banco)
    {
        var client = billet.Client;
        var pagador = new Pagador
        {
            CPFCNPJ = client.Cnpj.Value,
            Endereco = new Endereco()
            {
                Bairro = client.Address.Neighborhood,
                CEP = client.Address.ZipCode,
                Cidade = client.Address.City,
                LogradouroComplemento = client.Address.Complement,
                LogradouroEndereco = client.Address.Street,
                LogradouroNumero = client.Address.Number.ToString()
            },
            Nome = client.LegalName
        };
        
        var boleto = new Boleto(banco)
        {
            Pagador = pagador,
            DataEmissao = DateTime.Now,
            DataProcessamento = DateTime.Now,
            DataVencimento = billet.Installment.DueDate,
            ValorTitulo = billet.Installment.Amount,
            NossoNumero = nossoNumero.ToString(),
            EspecieDocumento = TipoEspecieDocumento.DM,
            Aceite = "N",
            CodigoInstrucao1 = "00",
            CodigoInstrucao2 = "00",
            DataDesconto = billetConfiguration.DiscountLimitDate,
            ValorDesconto = billetConfiguration.DiscountAmount,
            ValorAbatimento = billetConfiguration.RebateAmount,
            AvisoDebitoAutomaticoContaCorrente = "2",
            ImprimirValoresAuxiliares = true,
            ImprimirMensagemInstrucao = true
        };
        if (billetConfiguration.HasFine)
        {
            boleto.PercentualMulta = (decimal)billetConfiguration.FinePercentage!;
            boleto.DataMulta = billet.Installment.DueDate.AddDays(1);
        }

        StringBuilder msgCaixa = new StringBuilder();
        if (boleto.ValorDesconto > 0)
            msgCaixa.AppendLine($"Conceder desconto de {boleto.ValorDesconto.ToString("R$ ##,##0.00")} até {boleto.DataDesconto.ToString("dd/MM/yyyy")}. ");
        if (boleto.ValorMulta > 0)
            msgCaixa.AppendLine($"Cobrar multa de {boleto.ValorMulta.ToString("R$ ##,##0.00")} após o vencimento. ");
        if (boleto.ValorJurosDia > 0)
            msgCaixa.AppendLine($"Cobrar juros de {boleto.ValorJurosDia.ToString("R$ ##,##0.00")} por dia de atraso. ");
        boleto.MensagemInstrucoesCaixa = msgCaixa.ToString();
        
        return boleto;
    }
    
    public static Stream GenerateBilletFile(Billet billet)
    {
        var billetConfiguration = billet.Configuration;
        var client = billet.Client;
        var banco = Banco.Instancia(756);
        
        banco.Beneficiario = new Beneficiario
        {
            Codigo = billetConfiguration.CompanyCode,
            Nome = billetConfiguration.CompanyName,
            ContaBancaria = new ContaBancaria
            {
                Agencia = billetConfiguration.AgencyNumber,
                DigitoAgencia = "",
                Conta = billetConfiguration.AccountNumber,
                DigitoConta = billetConfiguration.AccountDigit,
                CarteiraPadrao = billetConfiguration.WalletNumber
            },
        };

        var pagador = new Pagador
        {
            CPFCNPJ = client.Cnpj.Value,
            Endereco = new Endereco()
            {
                Bairro = client.Address.Neighborhood,
                CEP = client.Address.ZipCode,
                Cidade = client.Address.City,
                LogradouroComplemento = client.Address.Complement,
                LogradouroEndereco = client.Address.Street,
                LogradouroNumero = client.Address.Number.ToString()
            },
            Nome = client.LegalName
        };
        
        var boleto = new Boleto(banco)
        {
            Pagador = pagador,
            DataEmissao = DateTime.Now,
            DataProcessamento = DateTime.Now,
            DataVencimento = billet.Installment.DueDate,
            ValorTitulo = billet.Installment.Amount,
            NossoNumero = billet.NossoNumero.ToString(),
            EspecieDocumento = TipoEspecieDocumento.DM,
            Aceite = "N",
            CodigoInstrucao1 = "00",
            CodigoInstrucao2 = "00",
            DataDesconto = billetConfiguration.DiscountLimitDate,
            ValorDesconto = billetConfiguration.DiscountAmount,
            ValorAbatimento = billetConfiguration.RebateAmount,
            AvisoDebitoAutomaticoContaCorrente = "2",
            ImprimirValoresAuxiliares = true,
            ImprimirMensagemInstrucao = true
        };
        if (billetConfiguration.HasFine)
        {
            boleto.PercentualMulta = (decimal)billetConfiguration.FinePercentage!;
            boleto.DataMulta = billet.Installment.DueDate.AddDays(1);
        }

        StringBuilder msgCaixa = new StringBuilder();
        if (boleto.ValorDesconto > 0)
            msgCaixa.AppendLine($"Conceder desconto de {boleto.ValorDesconto.ToString("R$ ##,##0.00")} até {boleto.DataDesconto.ToString("dd/MM/yyyy")}. ");
        if (boleto.ValorMulta > 0)
            msgCaixa.AppendLine($"Cobrar multa de {boleto.ValorMulta.ToString("R$ ##,##0.00")} após o vencimento. ");
        if (boleto.ValorJurosDia > 0)
            msgCaixa.AppendLine($"Cobrar juros de {boleto.ValorJurosDia.ToString("R$ ##,##0.00")} por dia de atraso. ");
        boleto.MensagemInstrucoesCaixa = msgCaixa.ToString();
        
        var boletoBancario = new BoletoBancario
        {
            
            Banco = banco,
            Boleto = boleto
        };

        var pdfHTML = boletoBancario.MontaHtmlEmbedded(false, true, null, (string) null);
        
        var pdf = new HtmlToPDF();
        var bytes = pdf.ReturnPDF(pdfHTML);

        var stream = new MemoryStream(bytes);

        return stream;
    }
 
}