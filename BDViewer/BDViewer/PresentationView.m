//
//  PresentationView.m
//  BDViewer
//
//  Created by Liz Dovey on 11-10-20.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import "PresentationView.h"
#import "BDPresentation.h"
#import "TherapyView.h"
#import "BDLinkedNoteAssociation.h"
#import "BDLinkedNote.h"

@interface PresentationView() 
-(NSString *)retrieveNoteForParent:(NSString *)theParentId forPropertyName:(NSString *)thePropertyName;
-(void)buildHTMLFromData;
-(void)loadHTMLIntoWebView;
@end

@implementation PresentationView
@synthesize parentId;
@synthesize parentName;
@synthesize dataTableView;
@synthesize dataWebView;
@synthesize presentationArray;
@synthesize overviewHTMLString;

-(id)initWithParentId:(NSString *)pParentId withParentName:(NSString *)pParentName
{
    self = [super initWithNibName:@"PresentationView" bundle:nil];
    if(self)
    {
        parentId = [pParentId retain];
        parentName = [pParentName retain];
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
    self.title = parentName;
}

-(void)viewWillAppear:(BOOL)animated
{
    [super viewWillAppear:animated];
    self.presentationArray = [NSArray arrayWithArray:[BDPresentation retrieveAllWithParentUUID:parentId]];
    self.overviewHTMLString = [self retrieveNoteForParent:(NSString *)parentId forPropertyName:@"Overview"];
    [self loadHTMLIntoWebView];
}

- (void)viewDidUnload
{
    parentId = nil;
    parentName = nil;
    [self setDataTableView:nil];
    [dataTableView release];
    dataTableView = nil;
    [self setDataWebView:nil];
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
    [dataTableView release];
    [presentationArray release];
    [parentId release];
    [parentName release];
    [dataWebView release];
    [overviewHTMLString release];
    [super dealloc];
}

#pragma mark - TableView DataSource

- (int)numberOfSectionsInTableView:(UITableView *)tableView {
    return 1;
}

- (int)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    return [self.presentationArray count];
}

#pragma mark - TableView Delegate

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    static NSString *CellIdentifier = @"Cell";
    
    UITableViewCell *cell = [dataTableView dequeueReusableCellWithIdentifier:CellIdentifier];
    if (cell == nil) {
        cell = [[[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:CellIdentifier] autorelease];
    }
    cell.textLabel.text = [[self.presentationArray objectAtIndex:indexPath.row] name ];
    cell.accessoryType = UITableViewCellAccessoryDisclosureIndicator;
    return cell;
}

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    BDPresentation *presentation = [presentationArray objectAtIndex:indexPath.row];
    TherapyView *vwTherapy = [[TherapyView alloc] initWithPresentationId:presentation.uuid withPresentationName:presentation.name];
    [self.navigationController pushViewController:vwTherapy animated:YES];
    [vwTherapy release];
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


-(void)buildHTMLFromData
{
    NSMutableString *tmpHTML = [[[NSMutableString alloc] initWithCapacity:0] autorelease];
    
    NSString *presentationOverview = [self retrieveNoteForParent:parentId forPropertyName:@"Overview"];
    if(presentationOverview != nil && [presentationOverview length] > 8) //  && ![presentationOverview isEqualToString:@"<p></p>"])
        [tmpHTML appendString: presentationOverview];
    
    self.overviewHTMLString = tmpHTML;
}


-(void)loadHTMLIntoWebView 
{
    if(self.overviewHTMLString != nil && [self.overviewHTMLString length] > 8)
    {
        NSURL *bundleURL = [[NSBundle mainBundle] bundleURL];
        
        [self.dataWebView loadHTMLString:[NSString stringWithFormat:@"<html><head><link rel=\"stylesheet\" type=\"text/css\" href=\"bdviewer.css\" /> </head><body>%@</body></html>",self.overviewHTMLString] baseURL:bundleURL];

        [self.dataWebView setBackgroundColor:[UIColor whiteColor]];
        [self.dataWebView setOpaque:YES];
    } else {
        dataTableView.tableHeaderView = nil;
    }
}
@end

