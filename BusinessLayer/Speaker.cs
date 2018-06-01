using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer
{

    public class Speaker
	{
        private const int minimunRequiredExperienceYears = 10;
        private const int minimumRequiredCertifications = 3;
        private const int minimumRequierdBrowserVersion = 9;

        public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public int? ExperienceYears { get; set; }
		public bool HasBlog { get; set; }
		public string BlogURL { get; set; }
		public WebBrowser Browser { get; set; }
		public List<string> Certifications { get; set; }
		public string Employer { get; set; }
		public int RegistrationFee { get; set; }
		public List<BusinessLayer.Session> Sessions { get; set; }

		public int? Register(IRepository repository)
		{

			int? speakerId = null;
			bool isGoodCandidate = false;
			bool isApproved = false;

			var oldTechnologies = new List<string>() { "Cobol", "Punch Cards", "Commodore", "VBScript" };
			var oldEmailDomains = new List<string>() { "aol.com", "hotmail.com", "prodigy.com", "CompuServe.com" };
            var bigEmployers = new List<string>() { "Microsoft", "Google", "Fog Creek Software", "37Signals" };

            if (!string.IsNullOrWhiteSpace(FirstName))
			{
				if (!string.IsNullOrWhiteSpace(LastName))
				{
					if (!string.IsNullOrWhiteSpace(Email))
                    {
                        isGoodCandidate = isGoodSpeakerCandidate(oldEmailDomains, bigEmployers);

                        if (isGoodCandidate)
                        {

                            if (Sessions.Count() != 0)
                            {
                                foreach (var session in Sessions)
                                {
                                    foreach (var technology in oldTechnologies)
                                    {
                                        if (session.Title.Contains(technology) || session.Description.Contains(technology))
                                        {
                                            session.Approved = false;
                                            break;
                                        }
                                        else
                                        {
                                            session.Approved = true;
                                            isApproved = true;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                throw new ArgumentException("Can't register speaker with no sessions to present.");
                            }

                            if (isApproved)
                            {
                                calculateRegistrationFee();

                                try
                                {
                                    speakerId = repository.SaveSpeaker(this);
                                }
                                catch (Exception e)
                                {

                                }
                            }
                            else
                            {
                                throw new NoSessionsApprovedException("No sessions approved.");
                            }
                        }
                        else
                        {
                            throw new SpeakerDoesntMeetRequirementsException("Speaker doesn't meet our abitrary and capricious standards.");
                        }
                    }
                    else
					{
						throw new ArgumentNullException("Email is required.");
					}
				}
				else
				{
					throw new ArgumentNullException("Last name is required.");
				}
			}
			else
			{
				throw new ArgumentNullException("First Name is required");
			}

			return speakerId;
		}

        private bool isGoodSpeakerCandidate(List<string> domains, List<string> bigEmployers)
        {
            string emailDomain = Email.Split('@').Last();
            bool good = ((ExperienceYears > minimunRequiredExperienceYears || HasBlog || Certifications.Count() > minimumRequiredCertifications || bigEmployers.Contains(Employer)));
            bool redFlags = (domains.Contains(emailDomain) && (Browser.Name == WebBrowser.BrowserName.InternetExplorer && Browser.MajorVersion < minimumRequierdBrowserVersion));

            return good || !redFlags;
            //|| conditional OR
            // | regular OR
        }

        private void calculateRegistrationFee()
        {
            if (ExperienceYears <= 1)
            {
                RegistrationFee = 500;
            }
            else if (ExperienceYears >= 2 && ExperienceYears <= 3)
            {
                RegistrationFee = 250;
            }
            else if (ExperienceYears >= 4 && ExperienceYears <= 5)
            {
                RegistrationFee = 100;
            }
            else if (ExperienceYears >= 6 && ExperienceYears <= 9)
            {
                RegistrationFee = 50;
            }
            else
            {
                RegistrationFee = 0;
            }
        }

        #region Custom Exceptions
        public class SpeakerDoesntMeetRequirementsException : Exception
		{
			public SpeakerDoesntMeetRequirementsException(string message)
				: base(message)
			{
			}

			public SpeakerDoesntMeetRequirementsException(string format, params object[] args)
				: base(string.Format(format, args)) { }
		}

		public class NoSessionsApprovedException : Exception
		{
			public NoSessionsApprovedException(string message)
				: base(message)
			{
			}
		}
		#endregion
	}
}