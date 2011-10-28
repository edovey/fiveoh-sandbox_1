//
//  TherapyView.m
//  BDViewer
//
//  Created by Liz Dovey on 11-10-20.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import "TherapyView.h"
#import "BDTherapyGroup.h"
#import "BDTherapy.h"
#import "BDPathogenGroup.h"
#import "BDPathogen.h"
#import "BDTherapyGroup.h"
#import "BDTherapy.h"
#import "BDPresentation.h"
#import "BDLinkedNote.h"
#import "BDLinkedNoteAssociation.h"

@interface TherapyView() 
-(NSString *)retrieveNoteForParent:(NSString *)theParentId forPropertyName:(NSString *)thePropertyName;
-(void)loadHTMLIntoWebView;
-(NSString *)buildHTMLFromData;
-(NSString *)buildPathogenGroupHTML:(BDPathogenGroup *)thePathogen;
-(NSString *)buildTherapyGroupHTML:(BDTherapyGroup *)theTherapyGroup;
-(NSString *)buildTherapyHTML:(BDTherapy *)theTherapy;

@end

@implementation TherapyView
@synthesize dataWebView;
@synthesize presentationId;
@synthesize presentationName;
@synthesize diseaseId;
@synthesize diseaseName;
@synthesize detailHTMLString;

-(id)initWithPresentationId:(NSString *)pPresentationId withPresentationName:(NSString *)pPresentationName
{
    self = [super initWithNibName:@"TherapyView" bundle:nil];
    if(self)
    {
        self.presentationId = [pPresentationId retain];
        self.presentationName = [pPresentationName retain];
        self.title = presentationName;
    }
    return self;
}

-(id)initWithDiseaseId:(NSString *)pDiseaseId withDiseaseName:(NSString *) pDiseaseName
{
    self = [super initWithNibName:@"TherapyView" bundle:nil];
    if(self)
    {
        self.diseaseId = [pDiseaseId retain];
        self.diseaseName = [pDiseaseName retain];
        self.title = diseaseName;
        
        if(self.diseaseId != nil && [self.diseaseId length] > 0)
        {           
            // get presentation id using disease record
            NSArray *presentationArray = [BDPresentation retrieveAllWithParentUUID:diseaseId];
            //TODO: bounds check on array
            self.presentationId = [[presentationArray objectAtIndex:0] uuid];
        }
    }
    return self;
}

- (void)didReceiveMemoryWarning
{
    // Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
    
    // Release any cached data, images, etc that aren't in use.
}

#pragma mark - View lifecycle

- (void)viewDidLoad
{
    [super viewDidLoad];

    // Do any additional setup after loading the view from its nib.
}

-(void)viewWillAppear:(BOOL)animated
{
    [super viewWillAppear:animated];
    detailHTMLString = [NSString stringWithString:[self buildHTMLFromData]];
    [self loadHTMLIntoWebView];
}

- (void)viewDidUnload
{
    [dataWebView release];
    dataWebView = nil;
    detailHTMLString = nil;
    presentationId = nil;
    presentationName = nil;
    diseaseId = nil;
    diseaseName = nil;
    [super viewDidUnload];
    // Release any retained subviews of the main view.
    // e.g. self.myOutlet = nil;
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    // Return YES for supported orientations
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

- (void)dealloc {
    [presentationId release];
    [presentationName release];
    [diseaseId release];
    [diseaseName release];
    [dataWebView release];
    [super dealloc];
}

#pragma mark - Private Class methods
-(NSString *)retrieveNoteForParent:(NSString *)theParentId forPropertyName:(NSString *)thePropertyName
{
    NSArray *lnAssociations = [BDLinkedNoteAssociation retrieveAllWithParentUUID:theParentId withPropertyName:thePropertyName];
    if([lnAssociations count] > 0)
    {
        BDLinkedNote *note = [BDLinkedNote retrieveWithUUID:[[lnAssociations objectAtIndex:0] linkedNoteId]];
        if ([note.documentText length] > 0)
            return note.documentText;
    }
    return nil;
}


-(NSString *)buildHTMLFromData
{
    NSMutableString *bodyHTML = [[[NSMutableString alloc] initWithCapacity:0] autorelease];
    if(diseaseId != nil && [diseaseId length] > 0)
    {
        NSString *diseaseOverview = [self retrieveNoteForParent:diseaseId forPropertyName:@"Overview"];
        if(diseaseOverview != nil && [diseaseOverview length] > 8) // && ![diseaseOverview isEqualToString:@"<p> </p>"])
            [bodyHTML appendString:diseaseOverview];
    }
    
    NSString *presentationOverview = [self retrieveNoteForParent:presentationId forPropertyName:@"Overview"];
    if(presentationOverview != nil && [presentationOverview length] > 8) //  && ![presentationOverview isEqualToString:@"<p> </p>"])
        [bodyHTML appendString: presentationOverview];
    
    
    // use presentationId to get pathogengroups
    NSArray *pathogenGroupArray = [BDPathogenGroup retrieveAllWithParentUUID:self.presentationId];
    
    for(BDPathogenGroup *pGroup in pathogenGroupArray) {
        [bodyHTML appendFormat:@"%@",[self buildPathogenGroupHTML:pGroup]];
        
        // use therapyGroup uuid to get therapies
        NSArray *therapyGroupArray = [BDTherapyGroup retrieveAllWithParentUUID:pGroup.uuid];

        // header for therapy section 
        [bodyHTML appendString:@"<h2>Recommended Empiric Therapy<h2>"];
        [bodyHTML appendString:@"<table class=\"therapy\" border=\"1\" cellspacing=\"0\" cellpadding=\"5\">"];

        for (BDTherapyGroup *tGroup in therapyGroupArray)
        {
            [bodyHTML appendFormat:@"%@",[self buildTherapyGroupHTML:tGroup]];
        
            // use therapygroupid to get therapies
            NSArray *therapyArray = [BDTherapy retrieveAllWithParentUUID:tGroup.uuid];
            
            [bodyHTML appendString:@"<tr><th>Therapy</th><th>Dosage</th><th>Duration</th></tr>"];
            for (BDTherapy *therapy in therapyArray)
            {
                [bodyHTML appendFormat:@"%@", [self buildTherapyHTML:therapy]];
            }
         }
        [bodyHTML appendString:@"</table>"];
     }
    return [NSString stringWithString:bodyHTML];
}

-(void)loadHTMLIntoWebView 
{
    NSURL *bundleURL = [[NSBundle mainBundle] bundleURL];
      
    [self.dataWebView loadHTMLString:[NSString stringWithFormat:@"<html><head><link rel=\"stylesheet\" type=\"text/css\" href=\"bdviewer.css\" /> </head><body>%@</body></html>",self.detailHTMLString] baseURL:bundleURL];
    [self.dataWebView setBackgroundColor:[UIColor clearColor]];
    [self.dataWebView setOpaque:NO];
}

-(NSString *)buildPathogenGroupHTML:(BDPathogenGroup *)thePathogenGroup
{
    NSMutableString *pathogenGroupHTML = [[NSMutableString alloc] initWithCapacity:0];
    NSArray *pathogenArray = [BDPathogen retrieveAllWithParentUUID:[thePathogenGroup uuid]];

    if([pathogenArray count] > 0)
        [pathogenGroupHTML appendString:@"<h2>Usual Pathogens</h2>"];

    for(BDPathogen *pathogen in pathogenArray)
    {
       [pathogenGroupHTML appendFormat:@"%@<br>",pathogen.name ];
    }
  
    NSString *returnString = [NSString stringWithString:pathogenGroupHTML];
    [pathogenGroupHTML release];
    return returnString;
}

-(NSString *)buildTherapyGroupHTML:(BDTherapyGroup *)theTherapyGroup
{
    NSMutableString *therapyGroupHTML = [[NSMutableString alloc] initWithCapacity:0];
    [therapyGroupHTML appendFormat:@"<tr><td colspan=\"3\" align=\"left\"><u>%@</></td></tr>",theTherapyGroup.name];

    NSString *returnString = [NSString stringWithString:therapyGroupHTML];
    [therapyGroupHTML release];
    return returnString;
}

-(NSString *)buildTherapyHTML:(BDTherapy *)theTherapy
{
    NSMutableString *therapyHTML = [[NSMutableString alloc] initWithCapacity:0];
     [therapyHTML appendFormat:@"<tr><td><b>%@</b></td>",theTherapy.name];
    
    if([theTherapy.dosage length] > 0)
        [therapyHTML appendFormat:@"<td>%@</td>",theTherapy.dosage];
    if([theTherapy.duration length] > 0)
        [therapyHTML appendFormat:@"<td>%@</td>",theTherapy.duration];
    
    
    [therapyHTML appendString:@"</tr>"];

    NSString *returnString = [NSString stringWithString:therapyHTML];
    [therapyHTML release];
    return returnString;
}

@end
