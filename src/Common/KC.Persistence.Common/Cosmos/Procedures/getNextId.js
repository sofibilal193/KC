// STORED PROCEDURE TO GET NEW AUTO INCREMENTED ID FOR NEW DOC
function getNextId(itemType) {
    const ERROR_CODE = {
        NotAccepted: 429
    };
    const DISCRIMINATOR = 'AutoNumber'
    const PARTITION_KEY = 'AutoNumber'

    var nextId = 1;
    var response = getContext().getResponse();

    var collection = getContext().getCollection();

    if (!itemType) throw new Error(ERROR_CODE, "Null/Empty item type.");

    // attempt to get doc containing auto-number sequence
    var filterQuery =
    {
        'query' : 'SELECT * FROM d where d.Discriminator = @discriminator and d.ItemType = @itemType',
        'parameters' : [
            {'name':'@discriminator', 'value':DISCRIMINATOR},
            {'name':'@itemType', 'value':itemType}
        ] 
    };    

    // Query documents and take 1st item.
    var isAccepted = collection.queryDocuments(
        collection.getSelfLink(),
        filterQuery, {}, 
    function (err, feed, options) {
        if (err) throw err;

        // Check the feed and if empty, set the body to 'no docs found', 
        // else take 1st element from feed
        if (!feed || !feed.length) {
            createAutoNumber();
            response.setBody(nextId);
        }
        else {
            nextId = feed[0].NextId;
            updateAutoNumber(feed[0])
            response.setBody(nextId);
        }
    });

    if (!isAccepted) throw new Error('The query was not accepted by the server.');

    function createAutoNumber() {
        var options = { disableAutomaticIdGeneration: false };
        var doc = {
            'Discriminator': 'AutoNumber',
            'PartitionKey': PARTITION_KEY,
            'ItemType': itemType,
            'NextId': 2
        }
        collection.createDocument(
            collection.getSelfLink(), doc, options, 
            function (err, item, options) {
                if (err) throw err;
            });
    }

    function updateAutoNumber(doc) {
        doc.NextId = doc.NextId + 1;
        collection.upsertDocument(
          collection.getSelfLink(),
          doc);
    }
}