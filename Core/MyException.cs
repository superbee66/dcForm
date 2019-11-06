using System;

namespace dCForm.Core
{
    public class DocDataException : Exception { }

    public class InterpreterLocationException : DocDataException
    {
        public override string Message {
            get { return "could not locate a DocDataInterpreter to process the data"; }
        }
    }
    public class ImportException : Exception { }

    //TODO:add token to express what doc actually failed
    public class PocosImportException : ImportException
    {
        public override string Message {
            get {
                return "Poco import has not resulted in a new Head DocRev to match. DocRev string conventions will place the most current DocRev at the top of a list when OrderByDescending is applied. There seems to be a more current DocRev in the data store then what is being imported.";
            }
        }
    }

    public class SubmissionException : Exception { }

    public class SubmitDeniedException : SubmissionException { }

    public class NoChangesSinceRenderedException : SubmitDeniedException
    {
        public override string Message {
            get {
                return "skipped, the document appears to have nothing modified; did you forget to open & fill this doc/form before you submitted?";
            }
        }
    }

    public class NoChangesSinceLastSubmitException : SubmitDeniedException
    {
        public override string Message {
            get {
                return "skipped, the prior submission contained identical information as this";
            }
        }
    }

    public class NoOverwriteOfPreviouslyApproveException : SubmitDeniedException
    {
        public override string Message {
            get {
                return "skipped, previously approved, form can not be submitted for updates";
            }
        }
    }
}