using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace MyCourse.Customizations.Identity;

public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
{
    public CustomClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        ClaimsIdentity identity = await base.GenerateClaimsAsync(user);
        identity.AddClaim(new Claim("FullName", user.FullName));

        // In vari punti dell'applicazione stiamo usando la policy "CourseAuthor"
        // che scatenerà una query al database per verificare se l'id dell'utente è uguale
        // all'id dell'autore del corso a cui sta cercando di accedere.
        // Potremmo evitare tale query se aggiungessimo un claim personalizzato contenente gli id dei suoi corsi.
        // Dopo aver ottenuto tali id dal database grazie al course service, li aggiungo come claim
        // identity.AddClaim(new Claim("AuthorOfCourses", "5,7,22"));

        return identity;
    }
}
