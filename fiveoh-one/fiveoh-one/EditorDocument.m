//
//  EditorDocument.m
//  fiveoh-one
//
//  Created by Karl Schulze on 2011-08-18.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import "EditorDocument.h"
#import "NSString+UUID.h"


@implementation EditorDocument

@dynamic uuid;
@dynamic createdDate;
@dynamic modifiedDate;
@dynamic documentText;
@dynamic createdBy;
@dynamic lastModifiedBy;
@dynamic inUseBy;

+(EditorDocument *)create
{
    NSManagedObjectContext *moc = [[DataController sharedInstance] managedObjectContext]; 
    NSEntityDescription *entity = [NSEntityDescription entityForName:@"EditorDocument" inManagedObjectContext:moc];

	EditorDocument *document = [[EditorDocument alloc] initWithEntity:entity insertIntoManagedObjectContext:moc];

    document.uuid = [NSString UUIDCreate];
    document.createdDate = [NSDate date];
    
    [[DataController sharedInstance] saveContext];
    return document;
}

+(EditorDocument *)retrieveWithUUID:(NSString *)theUUID
{
    return (EditorDocument *)[[DataController sharedInstance] retrieveManagedObject:@"EditorDocument" 
                                                                               uuid:theUUID 
                                                                          targetMOC:nil];
}

@end
