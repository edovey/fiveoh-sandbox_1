//
//  PrototypeView.m
//  BDViewer
//
//  Created by Karl Schulze on 2011-08-26.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import "PrototypeView.h"
#import "LinkedNote.h"
#import "RepositoryHandler.h"

@implementation PrototypeView

@synthesize documentArray = _documentArray;

@synthesize dataTableView;
@synthesize infoLabel;
@synthesize modifiedDateLabel;
@synthesize repositoryUrlTextField;
@synthesize documentTextTextView;

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        // Custom initialization
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

- (void)viewDidUnload
{
    [self setDataTableView:nil];
    [self setInfoLabel:nil];
    [self setRepositoryUrlTextField:nil];
    [self setDocumentTextTextView:nil];
    [self setModifiedDateLabel:nil];
    [super viewDidUnload];
    // Release any retained subviews of the main view.
    // e.g. self.myOutlet = nil;
}

- (void)viewWillAppear:(BOOL)animated
{
    [super viewWillAppear:animated];
    
    self.documentArray = [[DataController sharedInstance] allInstancesOf:ENTITYNAME_LINKEDNOTE 
                                                               orderedBy:nil 
                                                                loadData:NO 
                                                               targetMOC:nil];
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
	return YES;
}

- (IBAction)updateRowAction:(id)sender 
{
    NSString *uuid = self.infoLabel.text;
    NSPredicate *predicate = [NSPredicate predicateWithBlock:^BOOL(id evaluatedObject, NSDictionary *bindings) 
    {
        LinkedNote *linkeNote = evaluatedObject;
        return ([linkeNote.uuid isEqualToString:uuid]);
    } ];
    
    NSArray *documentArray = [self.documentArray filteredArrayUsingPredicate:predicate];
    if([documentArray count] > 0)
    {
        LinkedNote *document = [documentArray objectAtIndex:0];
        document.storageKey = self.repositoryUrlTextField.text;
        document.documentText = self.documentTextTextView.text;
        [document commitChanges];
    }
}

- (IBAction)pushAction:(id)sender 
{
    int count = [RepositoryHandler pushQueue];
    NSLog(@"Queue entries processed: %d", count);
}

- (IBAction)pullAction:(id)sender 
{
    [RepositoryHandler pullLatest];
    
    self.documentArray = [[DataController sharedInstance] allInstancesOf:ENTITYNAME_LINKEDNOTE 
                                                               orderedBy:nil 
                                                                loadData:NO 
                                                               targetMOC:nil];

    [self.dataTableView reloadData];
}

- (IBAction)createAction:(id)sender 
{
    NSString *uuid = [LinkedNote create];
    NSLog(@"new uuid %@", uuid);
    
    self.documentArray = [[DataController sharedInstance] allInstancesOf:ENTITYNAME_LINKEDNOTE 
                                                               orderedBy:nil 
                                                                loadData:NO 
                                                               targetMOC:nil];
    [self.dataTableView reloadData];
}

#pragma mark - Table view data source

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView
{
    // Return the number of sections.
    return 1;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    // Return the number of rows in the section.
    return [self.documentArray count];
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    static NSString *CellIdentifier = @"Cell";
    
    UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:CellIdentifier];
    if (cell == nil) {
        cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:CellIdentifier];
    }
    
    // Configure the cell...
    LinkedNote *entry = [self.documentArray objectAtIndex:indexPath.row];
    cell.textLabel.text = entry.uuid;
    
    return cell;
}

#pragma mark - Table view delegate

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    NSDateFormatter* dateFormatter = [[NSDateFormatter alloc] init];
	[dateFormatter setTimeStyle:NSDateFormatterFullStyle];
	[dateFormatter setDateFormat:DATETIMEFORMAT];

    LinkedNote *entry = [self.documentArray objectAtIndex:indexPath.row];
    self.infoLabel.text = entry.uuid;
    self.repositoryUrlTextField.text = entry.storageKey;
    self.documentTextTextView.text = entry.documentText;
    self.modifiedDateLabel.text = [dateFormatter stringFromDate:entry.modifiedDate];
}

@end
