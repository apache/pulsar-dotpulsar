#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using pbr = Google.Protobuf.Reflection;

public enum ServerError {
    [pbr::OriginalName("UnknownError")] UnknownError = 0,
    /// <summary>
    /// Error with ZK/metadata
    /// </summary>
    [pbr::OriginalName("MetadataError")] MetadataError = 1,
    /// <summary>
    /// Error writing reading from BK
    /// </summary>
    [pbr::OriginalName("PersistenceError")] PersistenceError = 2,
    /// <summary>
    /// Non valid authentication
    /// </summary>
    [pbr::OriginalName("AuthenticationError")] AuthenticationError = 3,
    /// <summary>
    /// Not authorized to use resource
    /// </summary>
    [pbr::OriginalName("AuthorizationError")] AuthorizationError = 4,
    /// <summary>
    /// Unable to subscribe/unsubscribe because
    /// </summary>
    [pbr::OriginalName("ConsumerBusy")] ConsumerBusy = 5,
    /// <summary>
    /// other consumers are connected
    /// </summary>
    [pbr::OriginalName("ServiceNotReady")] ServiceNotReady = 6,
    /// <summary>
    /// Unable to create producer because backlog quota exceeded
    /// </summary>
    [pbr::OriginalName("ProducerBlockedQuotaExceededError")] ProducerBlockedQuotaExceededError = 7,
    /// <summary>
    /// Exception while creating producer because quota exceeded
    /// </summary>
    [pbr::OriginalName("ProducerBlockedQuotaExceededException")] ProducerBlockedQuotaExceededException = 8,
    /// <summary>
    /// Error while verifying message checksum
    /// </summary>
    [pbr::OriginalName("ChecksumError")] ChecksumError = 9,
    /// <summary>
    /// Error when an older client/version doesn't support a required feature
    /// </summary>
    [pbr::OriginalName("UnsupportedVersionError")] UnsupportedVersionError = 10,
    /// <summary>
    /// Topic not found
    /// </summary>
    [pbr::OriginalName("TopicNotFound")] TopicNotFound = 11,
    /// <summary>
    /// Subscription not found
    /// </summary>
    [pbr::OriginalName("SubscriptionNotFound")] SubscriptionNotFound = 12,
    /// <summary>
    /// Consumer not found
    /// </summary>
    [pbr::OriginalName("ConsumerNotFound")] ConsumerNotFound = 13,
    /// <summary>
    /// Error with too many simultaneously request
    /// </summary>
    [pbr::OriginalName("TooManyRequests")] TooManyRequests = 14,
    /// <summary>
    /// The topic has been terminated
    /// </summary>
    [pbr::OriginalName("TopicTerminatedError")] TopicTerminatedError = 15,
    /// <summary>
    /// Producer with same name is already connected
    /// </summary>
    [pbr::OriginalName("ProducerBusy")] ProducerBusy = 16,
    /// <summary>
    /// The topic name is not valid
    /// </summary>
    [pbr::OriginalName("InvalidTopicName")] InvalidTopicName = 17,
    /// <summary>
    /// Specified schema was incompatible with topic schema
    /// </summary>
    [pbr::OriginalName("IncompatibleSchema")] IncompatibleSchema = 18,
    /// <summary>
    /// Dispatcher assign consumer error
    /// </summary>
    [pbr::OriginalName("ConsumerAssignError")] ConsumerAssignError = 19,
    /// <summary>
    /// Transaction coordinator not found error
    /// </summary>
    [pbr::OriginalName("TransactionCoordinatorNotFound")] TransactionCoordinatorNotFound = 20,
    /// <summary>
    /// Invalid txn status error
    /// </summary>
    [pbr::OriginalName("InvalidTxnStatus")] InvalidTxnStatus = 21,
    /// <summary>
    /// Not allowed error
    /// </summary>
    [pbr::OriginalName("NotAllowedError")] NotAllowedError = 22,
    /// <summary>
    /// Ack with transaction conflict
    /// </summary>
    [pbr::OriginalName("TransactionConflict")] TransactionConflict = 23,
    /// <summary>
    /// Transaction not found
    /// </summary>
    [pbr::OriginalName("TransactionNotFound")] TransactionNotFound = 24,
    /// <summary>
    /// When a producer asks and fail to get exclusive producer access,
    /// </summary>
    [pbr::OriginalName("ProducerFenced")] ProducerFenced = 25,
}
