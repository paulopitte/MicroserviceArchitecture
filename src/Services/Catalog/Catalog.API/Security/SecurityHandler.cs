
using BuildingBlocks.Security;

namespace Catalog.API.Security;


public record SecurityCipherCommand(string Pass) : ICommand<SecurityCipherResult>;
public record SecurityDeCipherCommand(string Pass) : ICommand<SecurityCipherResult>;

public record SecurityCipherResult(string PassCipher);


public class SecurityCipherCommandValidation : AbstractValidator<SecurityCipherCommand>
{
    public SecurityCipherCommandValidation()
    {
        RuleFor(x => x.Pass).NotEmpty().WithMessage("Pass is required");
    }
}



public class SecurityHandler
    : ICommandHandler<SecurityCipherCommand, SecurityCipherResult>,
     ICommandHandler<SecurityDeCipherCommand, SecurityCipherResult>
{
    public async Task<SecurityCipherResult> Handle(SecurityCipherCommand request, CancellationToken cancellationToken)
    {
        var result = AESCipher.Cipher(request.Pass);

        return await Task.FromResult(new SecurityCipherResult(result));
    }

    public async Task<SecurityCipherResult> Handle(SecurityDeCipherCommand request, CancellationToken cancellationToken)
    {
        var result = AESCipher.Decipher(request.Pass);

        return await Task.FromResult(new SecurityCipherResult(result));
    }
}

